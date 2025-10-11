using Components.Player.Misc;
using Core.Misc;
using Core.Utilities.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Utilities;

namespace Systems.Initialization.Player.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct SpawnCommandsExecutorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new PlayerSpawnCommandList
                {
                    Value = new(5, Allocator.Persistent),
                });

            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandList = SystemAPI.GetSingleton<PlayerSpawnCommandList>();
            int length = commandList.Value.Length;

            if (length == 0) return;

            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            for (int i = 0; i < length; i++)
            {
                var command = commandList.Value[i];
                var newEntity = state.EntityManager.Instantiate(command.Entity);

                this.GetPosOnGround(ref state, in physicsWorld, in command, out var posOnGround);
                SystemAPI.SetComponent(newEntity, LocalTransform.FromPosition(posOnGround.Add(y: command.GameEntitySize.ObjectHeight)));
            }

            commandList.Value.Clear();
        }

        [BurstCompile]
        private void GetPosOnGround(
            ref SystemState state
            , in PhysicsWorldSingleton physicsWorld
            , in SpawnCommand command
            , out float3 posOnGround)
        {
            posOnGround = default;
            float3 startPos = SystemAPI.GetComponent<LocalTransform>(command.Entity).Position;
            startPos.y = 100f;

            bool hit = this.CastRayToGround(in physicsWorld, in startPos, out var raycastHit);
            if (!hit) return;

            posOnGround = raycastHit.Position;
        }

        [BurstCompile]
        private bool CastRayToGround(
            in PhysicsWorldSingleton physicsWorld
            , in float3 startPos
            , out Unity.Physics.RaycastHit raycastHit)
        {
            float3 rayStart = startPos;
            float3 rayEnd = startPos.Add(y: -500f);

            RaycastInput raycastInput = new()
            {
                Start = rayStart,
                End = rayEnd,
                Filter = new CollisionFilter
                {
                    BelongsTo = (uint)CollisionLayer.Ground,
                    CollidesWith = (uint)CollisionLayer.Ground,
                },
            };

            return physicsWorld.CastRay(raycastInput, out raycastHit);
        }

    }

}
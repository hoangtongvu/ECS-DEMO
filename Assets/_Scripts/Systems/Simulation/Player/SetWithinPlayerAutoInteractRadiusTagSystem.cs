using Unity.Burst;
using Unity.Entities;
using Components.Player;
using Unity.Mathematics;
using Components.Misc;
using Unity.Physics;
using Unity.Collections;
using Unity.Transforms;
using Core.Misc;

namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct SetWithinPlayerAutoInteractRadiusTagSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , LocalTransform>()
                .Build();

            state.RequireForUpdate(entityQuery);
            state.RequireForUpdate<WithinPlayerAutoInteractRadiusTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var hitList = new NativeList<DistanceHit>(Allocator.Temp);
            half autoInteractRadius = SystemAPI.GetSingleton<AutoInteractRadius>().Value;

            // Clear tags
            foreach (var withinPlayerAutoInteractRadiusTag in
                SystemAPI.Query<
                    EnabledRefRW<WithinPlayerAutoInteractRadiusTag>>())
            {
                withinPlayerAutoInteractRadiusTag.ValueRW = false;
            }

            foreach (var transformRef in
                SystemAPI.Query<
                    RefRO<LocalTransform>>()
                    .WithAll<PlayerTag>())
            {
                bool hasHit = physicsWorld.OverlapSphere(
                    transformRef.ValueRO.Position
                    , autoInteractRadius
                    , ref hitList
                    , new CollisionFilter
                    {
                        BelongsTo = (uint)CollisionLayer.Player,
                        CollidesWith = (uint)CollisionLayer.Default,
                    });

                if (!hasHit) continue;

                int length = hitList.Length;
                for (int i = 0; i < length; i++)
                {
                    var hitEntity = hitList[i].Entity;
                    if (!SystemAPI.HasComponent<WithinPlayerAutoInteractRadiusTag>(hitEntity)) continue;
                    SystemAPI.SetComponentEnabled<WithinPlayerAutoInteractRadiusTag>(hitEntity, true);

                }

            }

            hitList.Dispose();

        }

    }

}
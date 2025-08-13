using Components.CustomIdentification;
using Components.MyCamera;
using Components.Player;
using Core.CustomIdentification;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation.MyCamera
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct CamMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQuery camQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UniqueIdICD
                    , LocalTransform
                    , AddPos>()
                .Build();

            EntityQuery playerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , LocalTransform>()
                .Build();

            state.RequireForUpdate(camQuery);
            state.RequireForUpdate(playerQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Get Player Pos through Singleton/Query.
            // Pass Player Pos into MoveJob to calculate and set Cam Pos.
            float3 playerPos = float3.zero;

            foreach (var playerTransformRef in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PlayerTag>())
            {
                playerPos = playerTransformRef.ValueRO.Position;
            }

            new MoveJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                PlayerPos = playerPos,
            }.ScheduleParallel();

        }

        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            public float deltaTime;
            public float3 PlayerPos;
            
            private void Execute(
                in UniqueIdICD id
                , ref LocalTransform transform
                , in AddPos addPos)
            {
                if (id.BaseId.Kind != UniqueKind.Camera) return;
                transform = transform.WithPosition(this.PlayerPos + addPos.Value);
            }

        }

    }

}
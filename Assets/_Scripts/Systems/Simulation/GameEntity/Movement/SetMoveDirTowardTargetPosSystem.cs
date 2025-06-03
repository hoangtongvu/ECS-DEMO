using Components.GameEntity.Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation.GameEntity.Movement
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct SetMoveDirTowardTargetPosSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , LocalTransform
                    , MoveDirectionFloat2
                    , CurrentWorldWaypoint>()
                .Build();

            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new SetMoveDirJob().ScheduleParallel();
        }

        [WithAll(typeof(CanMoveEntityTag))]
        [BurstCompile]
        private partial struct SetMoveDirJob : IJobEntity
        {
            private void Execute(
                in LocalTransform transform
                , ref MoveDirectionFloat2 moveDir
                , in CurrentWorldWaypoint currentWorldWaypoint
            )
            {
                float3 float3Dir = currentWorldWaypoint.Value - transform.Position;
                float2 float2Dir = new(float3Dir.x, float3Dir.z);
                float2 normalizedDir = math.normalize(float2Dir);
                moveDir.Value.x = normalizedDir.x;
                moveDir.Value.y = normalizedDir.y;
            }

        }

    }

}
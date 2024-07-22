using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


namespace Systems.Simulation.Unit
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct SetCurrentDisToTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    MoveableState
                    , LocalTransform
                    , DistanceToTarget
                    , TargetPosition>()
                .Build();

            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new SetCurrDis().ScheduleParallel();
        }

        [BurstCompile]
        private partial struct SetCurrDis : IJobEntity
        {
            private void Execute(
                in MoveableState moveableState
                , in LocalTransform transform
                , ref DistanceToTarget distanceToTarget
                , in TargetPosition targetPosition
            )
            {
                var deltaX = transform.Position.x - targetPosition.Value.x;
                var deltaY = transform.Position.z - targetPosition.Value.z;
                distanceToTarget.CurrentDistance = math.sqrt(deltaX * deltaX + deltaY * deltaY);
            }
        }
    }
}


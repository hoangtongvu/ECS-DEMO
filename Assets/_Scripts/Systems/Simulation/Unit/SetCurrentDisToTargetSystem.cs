using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Utilities.Helpers;


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
                distanceToTarget.CurrentDistance = MathHelper.GetDistance2(in transform.Position, in targetPosition.Value);
            }
        }
    }
}


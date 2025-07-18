using Components.GameEntity.Movement;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.Movement
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct ResetTargetSpeedOnStopMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , TargetMoveSpeed>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new SetTargetSpeedJob().ScheduleParallel();
        }

        [WithDisabled(typeof(CanMoveEntityTag))]
        [BurstCompile]
        private partial struct SetTargetSpeedJob : IJobEntity
        {
            private void Execute(
                ref TargetMoveSpeed targetMoveSpeed)
            {
                targetMoveSpeed.Value = 0;
            }

        }

    }

}
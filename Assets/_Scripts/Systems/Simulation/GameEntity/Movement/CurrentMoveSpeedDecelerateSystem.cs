using Components.GameEntity.Movement;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Simulation.GameEntity.Movement
{
    [UpdateInGroup(typeof(AccelerateMoveSpeedSystemGroup))]
    [BurstCompile]
    public partial struct CurrentMoveSpeedDecelerateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , CurrentMoveSpeed
                    , TargetMoveSpeed
                    , DecelerationValue>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new AccelerateJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel();
        }

        [WithDisabled(typeof(CanMoveEntityTag))]
        [BurstCompile]
        private partial struct AccelerateJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;

            private void Execute(
                ref CurrentMoveSpeed currentMoveSpeed
                , in TargetMoveSpeed targetMoveSpeed
                , DecelerationValue decelerationValue)
            {
                if (currentMoveSpeed.Value == 0) return;

                float speedDiff = targetMoveSpeed.Value - currentMoveSpeed.Value;
                float maxStep = decelerationValue.Value * DeltaTime;

                // Clamp the speed change to not overshoot
                float speedChange = math.clamp(speedDiff, -maxStep, maxStep);

                currentMoveSpeed.Value += speedChange;
            }

        }

    }

}
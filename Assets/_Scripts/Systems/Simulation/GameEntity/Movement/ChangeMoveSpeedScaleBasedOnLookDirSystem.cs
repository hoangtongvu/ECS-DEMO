using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Simulation.GameEntity.Movement
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(MoveWithCurrentSpeedSystem))]
    [BurstCompile]
    public partial struct ChangeMoveSpeedScaleBasedOnLookDirSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LookDirectionXZ
                    , MoveDirectionFloat2
                    , MoveSpeedScale>()
                .WithAll<
                    CanMoveEntityTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new ChangeMoveSpeedScaleJob().ScheduleParallel();
        }

        [WithAll(typeof(CanMoveEntityTag))]
        [BurstCompile]
        private partial struct ChangeMoveSpeedScaleJob : IJobEntity
        {
            private void Execute(
                in LookDirectionXZ lookDirectionXZ
                , in MoveDirectionFloat2 moveDirectionFloat2
                , ref MoveSpeedScale accelerationScale)
            {
                float dot = math.dot(lookDirectionXZ.Value, moveDirectionFloat2.Value);
                const float tempSpeedScale = 0.6f;

                accelerationScale.Value = dot >= 0
                    ? MoveSpeedScale.DefaultValue.Value
                    : tempSpeedScale;

            }

        }

    }

}
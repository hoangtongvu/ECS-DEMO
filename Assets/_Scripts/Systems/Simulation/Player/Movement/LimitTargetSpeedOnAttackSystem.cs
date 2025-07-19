using Components.GameEntity.Movement;
using Components.GameEntity.Reaction;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.Player.Movement
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct LimitTargetSpeedOnAttackSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    AttackReaction.StartedTag
                    , TargetMoveSpeed>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new SetTargetSpeedJob().ScheduleParallel();
        }

        [WithAll(typeof(AttackReaction.StartedTag))]
        [BurstCompile]
        private partial struct SetTargetSpeedJob : IJobEntity
        {
            [BurstCompile]
            void Execute(
                ref TargetMoveSpeed targetMoveSpeed)
            {
                const float targetMoveSpeedLimit = 2f;
                targetMoveSpeed.Value = targetMoveSpeedLimit;
            }

        }

    }

}
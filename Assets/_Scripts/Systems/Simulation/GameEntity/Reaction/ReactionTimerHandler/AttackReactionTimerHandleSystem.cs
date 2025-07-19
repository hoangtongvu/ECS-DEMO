using Components.GameEntity.Reaction;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.Reaction.ReactionTimerTickSystems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct AttackReactionTimerHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    AttackReaction.UpdatingTag
                    , AttackReaction.TimerSeconds>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new ResetTimerJob().ScheduleParallel();

            new TimerTickJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel();
        }

        [WithAll(typeof(AttackReaction.EndedTag))]
        [BurstCompile]
        private partial struct ResetTimerJob : IJobEntity
        {
            [BurstCompile]
            void Execute(ref AttackReaction.TimerSeconds timerSeconds)
            {
                timerSeconds.Value = 0;
            }

        }

        [WithAll(typeof(AttackReaction.UpdatingTag))]
        [BurstCompile]
        private partial struct TimerTickJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;

            [BurstCompile]
            void Execute(ref AttackReaction.TimerSeconds timerSeconds)
            {
                timerSeconds.Value += this.DeltaTime;
            }

        }

    }

}
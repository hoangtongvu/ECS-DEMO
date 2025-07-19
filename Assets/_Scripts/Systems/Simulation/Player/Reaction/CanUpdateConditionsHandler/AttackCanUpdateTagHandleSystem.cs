using Components.GameEntity.Attack;
using Components.GameEntity.Damage;
using Components.GameEntity.Reaction;
using Components.Misc;
using Components.Player;
using Systems.Simulation.GameEntity.Reaction.CanUpdateConditionsHandler;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.Player.Reaction.CanUpdateConditionsHandler
{
    [UpdateInGroup(typeof(CanUpdateConditionsHandleSystemGroup))]
    [BurstCompile]
    public partial struct AttackCanUpdateTagHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    AttackReaction.CanUpdateTag
                    , AttackReaction.UpdatingTag
                    , AttackReaction.TimerSeconds>()
                .WithAll<
                    IsAliveTag>()
                .WithAll<
                    PlayerTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<InputData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            bool hardwareInputState = SystemAPI.GetSingleton<InputData>().LeftMouseData.Down;

            foreach (var (reactionCanUpdateTag, reactionUpdatingTag, reactionTimerSecondsRef, attackDurationSecondsRef, isAliveTag) in SystemAPI
                .Query<
                    EnabledRefRW<AttackReaction.CanUpdateTag>
                    , EnabledRefRO<AttackReaction.UpdatingTag>
                    , RefRO<AttackReaction.TimerSeconds>
                    , RefRO<AttackDurationSeconds>
                    , EnabledRefRO<IsAliveTag>>()
                .WithAll<
                    PlayerTag>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                bool timedOut = reactionTimerSecondsRef.ValueRO.Value >= attackDurationSecondsRef.ValueRO.Value;

                reactionCanUpdateTag.ValueRW = isAliveTag.ValueRO
                    && (hardwareInputState || (reactionUpdatingTag.ValueRO && !timedOut));
            }

        }

    }

}
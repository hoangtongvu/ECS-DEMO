using Components.GameEntity.Damage;
using Components.GameEntity.Movement;
using Components.GameEntity.Reaction;
using Components.Player;
using Components.Player.Misc;
using DReaction;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.Player.Reaction.CanUpdateConditionsHandler
{
    [UpdateInGroup(typeof(CanUpdateConditionsHandleSystemGroup))]
    [UpdateAfter(typeof(RunCanUpdateTagHandleSystem))]
    [UpdateAfter(typeof(AttackCanUpdateTagHandleSystem))]
    [BurstCompile]
    public partial struct WalkCanUpdateTagHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    WalkReaction.CanUpdateTag>()
                .WithAll<
                    AttackReaction.CanUpdateTag
                    , IsAliveTag
                    , CanMoveEntityTag
                    , CurrentMoveSpeed
                    , PlayerReactionConfigsHolder>()
                .WithAll<
                    PlayerTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new TagHandleJob().ScheduleParallel(state.Dependency);
        }

        [WithAll(typeof(PlayerTag))]
        [WithAll(typeof(IsAliveTag))]
        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct TagHandleJob : IJobEntity
        {
            [BurstCompile]
            void Execute(
                EnabledRefRW<WalkReaction.CanUpdateTag> reactionCanUpdateTag
                , EnabledRefRO<AttackReaction.CanUpdateTag> attackCanUpdateTag
                , EnabledRefRO<CanMoveEntityTag> canMoveEntityTag
                , EnabledRefRO<RunReaction.CanUpdateTag> canRunUpdateTag)
            {
                reactionCanUpdateTag.ValueRW = canMoveEntityTag.ValueRO 
                    && !canRunUpdateTag.ValueRO && !attackCanUpdateTag.ValueRO;
            }

        }

    }

}
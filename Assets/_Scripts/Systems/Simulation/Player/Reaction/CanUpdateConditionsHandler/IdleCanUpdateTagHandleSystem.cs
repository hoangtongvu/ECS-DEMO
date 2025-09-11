using Unity.Entities;
using Unity.Burst;
using Components.GameEntity.Movement;
using Components.GameEntity.Damage;
using Components.Player;
using Components.GameEntity.Reaction;
using DReaction;

namespace Systems.Simulation.Player.Reaction.CanUpdateConditionsHandler
{
    [UpdateInGroup(typeof(CanUpdateConditionsHandleSystemGroup))]
    [UpdateAfter(typeof(AttackCanUpdateTagHandleSystem))]
    [BurstCompile]
    public partial struct IdleCanUpdateTagHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    IdleReaction.CanUpdateTag>()
                .WithAll<
                    AttackReaction.CanUpdateTag
                    , IsAliveTag
                    , CanMoveEntityTag>()
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
                EnabledRefRW<IdleReaction.CanUpdateTag> reactionCanUpdateTag
                , EnabledRefRO<AttackReaction.CanUpdateTag> attackCanUpdateTag
                , EnabledRefRO<CanMoveEntityTag> canMoveEntityTag)
            {
                reactionCanUpdateTag.ValueRW = !canMoveEntityTag.ValueRO && !attackCanUpdateTag.ValueRO;
            }

        }

    }

}
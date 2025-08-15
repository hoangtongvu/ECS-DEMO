using Components.GameEntity.Damage;
using Components.GameEntity.Movement;
using Components.GameEntity.Reaction;
using Components.Unit.Misc;
using Components.Unit.Reaction;
using DReaction;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.Unit.Reaction.CanUpdateConditionsHandler
{
    [UpdateInGroup(typeof(CanUpdateConditionsHandleSystemGroup))]
    [UpdateAfter(typeof(WorkCanUpdateTagHandleSystem))]
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
                    WorkReaction.CanUpdateTag
                    , IsAliveTag
                    , CanMoveEntityTag>()
                .WithAll<
                    UnitTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new TagHandleJob().ScheduleParallel(state.Dependency);
        }

        [WithAll(typeof(UnitTag))]
        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct TagHandleJob : IJobEntity
        {
            [BurstCompile]
            void Execute(
                EnabledRefRW<IdleReaction.CanUpdateTag> reactionCanUpdateTag
                , EnabledRefRO<WorkReaction.CanUpdateTag> workCanUpdateTag
                , EnabledRefRO<IsAliveTag> isAliveTag
                , EnabledRefRO<CanMoveEntityTag> canMoveEntityTag)
            {
                reactionCanUpdateTag.ValueRW = isAliveTag.ValueRO && !canMoveEntityTag.ValueRO && !workCanUpdateTag.ValueRO;
            }

        }

    }

}
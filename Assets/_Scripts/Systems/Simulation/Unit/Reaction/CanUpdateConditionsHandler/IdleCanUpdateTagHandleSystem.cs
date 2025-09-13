using Components.GameEntity.Damage;
using Components.GameEntity.Interaction;
using Components.GameEntity.Movement;
using Components.GameEntity.Reaction;
using Components.Unit.Misc;
using DReaction;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.Unit.Reaction.CanUpdateConditionsHandler
{
    [UpdateInGroup(typeof(CanUpdateConditionsHandleSystemGroup))]
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
                    InteractingEntity
                    , IsAlive
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
        [WithAll(typeof(IsAlive))]
        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct TagHandleJob : IJobEntity
        {
            [BurstCompile]
            void Execute(
                EnabledRefRW<IdleReaction.CanUpdateTag> reactionCanUpdateTag
                , EnabledRefRO<CanMoveEntityTag> canMoveEntityTag
                , in InteractingEntity interactingEntity)
            {
                bool isInteracting = interactingEntity.Value != Entity.Null;
                reactionCanUpdateTag.ValueRW = !canMoveEntityTag.ValueRO && !isInteracting;
            }

        }

    }

}
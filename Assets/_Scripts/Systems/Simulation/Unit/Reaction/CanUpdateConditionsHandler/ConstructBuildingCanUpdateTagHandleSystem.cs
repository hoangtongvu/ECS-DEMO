using Components.GameEntity.Damage;
using Components.GameEntity.Interaction;
using Components.GameEntity.Movement;
using Components.GameEntity.Reaction;
using Components.Unit.Misc;
using Core.GameEntity;
using DReaction;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.Unit.Reaction.CanUpdateConditionsHandler
{
    [UpdateInGroup(typeof(CanUpdateConditionsHandleSystemGroup))]
    [BurstCompile]
    public partial struct ConstructBuildingCanUpdateTagHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ConstructBuildingReaction.CanUpdateTag>()
                .WithAll<
                    InteractingEntity
                    , InteractionTypeICD
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
        [WithAll(typeof(IsAliveTag))]
        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct TagHandleJob : IJobEntity
        {
            [BurstCompile]
            void Execute(
                EnabledRefRW<ConstructBuildingReaction.CanUpdateTag> reactionCanUpdateTag
                , EnabledRefRO<CanMoveEntityTag> canMoveEntityTag
                , in InteractingEntity interactingEntity
                , in InteractionTypeICD interactionTypeICD)
            {
                bool isInteractingEntityValid = interactingEntity.Value != Entity.Null;
                bool isInteractionTypeConstructBuilding = interactionTypeICD.Value == InteractionType.ConstructBuilding;
                reactionCanUpdateTag.ValueRW = !canMoveEntityTag.ValueRO && isInteractingEntityValid && isInteractionTypeConstructBuilding;
            }

        }

    }

}
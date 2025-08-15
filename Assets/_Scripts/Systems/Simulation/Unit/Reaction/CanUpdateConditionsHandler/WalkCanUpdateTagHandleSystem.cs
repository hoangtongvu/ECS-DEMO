using Components.GameEntity.Damage;
using Components.GameEntity.Interaction;
using Components.GameEntity.Movement;
using Components.GameEntity.Reaction;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.Reaction;
using DReaction;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Simulation.Unit.Reaction.CanUpdateConditionsHandler
{
    [UpdateInGroup(typeof(CanUpdateConditionsHandleSystemGroup))]
    [UpdateAfter(typeof(RunCanUpdateTagHandleSystem))]
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
                    InteractingEntity
                    , IsAliveTag
                    , CanMoveEntityTag
                    , UnitProfileIdHolder
                    , MoveSpeedLinear>()
                .WithAll<
                    UnitTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<UnitReactionConfigsMap>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var unitReactionConfigsMap = SystemAPI.GetSingleton<UnitReactionConfigsMap>();

            state.Dependency = new TagHandleJob()
            {
                UnitReactionConfigsMap = unitReactionConfigsMap,
            }.ScheduleParallel(state.Dependency);
        }

        [WithAll(typeof(UnitTag))]
        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct TagHandleJob : IJobEntity
        {
            [ReadOnly] public UnitReactionConfigsMap UnitReactionConfigsMap;

            [BurstCompile]
            void Execute(
                EnabledRefRW<WalkReaction.CanUpdateTag> reactionCanUpdateTag
                , EnabledRefRO<IsAliveTag> isAliveTag
                , EnabledRefRO<CanMoveEntityTag> canMoveEntityTag
                , in UnitProfileIdHolder unitProfileIdHolder
                , in MoveSpeedLinear moveSpeedLinear
                , in InteractingEntity interactingEntity)
            {
                bool isInteracting = interactingEntity.Value != Entity.Null;

                if (!this.UnitReactionConfigsMap.Value.TryGetValue(unitProfileIdHolder.Value, out var unitReactionConfigs))
                    throw new KeyNotFoundException($"{nameof(UnitReactionConfigsMap)} does not contains key: {unitProfileIdHolder.Value}");

                bool currentSpeedIsWalkSpeed = moveSpeedLinear.Value == unitReactionConfigs.UnitWalkSpeed;
                reactionCanUpdateTag.ValueRW =
                    isAliveTag.ValueRO && canMoveEntityTag.ValueRO &&
                    !isInteracting && currentSpeedIsWalkSpeed;
            }

        }

    }

}
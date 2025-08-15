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
    [UpdateAfter(typeof(WorkCanUpdateTagHandleSystem))]
    [BurstCompile]
    public partial struct PatrolCanUpdateTagHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PatrolReaction.CanUpdateTag>()
                .WithAll<
                    InteractingEntity
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
                EnabledRefRW<PatrolReaction.CanUpdateTag> reactionCanUpdateTag
                , EnabledRefRO<WorkReaction.CanUpdateTag> workCanUpdateTag
                , EnabledRefRO<IsAliveTag> isAliveTag
                , in IdleReaction.TimerSeconds idleTimerSeconds
                , in UnitProfileIdHolder unitProfileIdHolder)
            {
                if (!this.UnitReactionConfigsMap.Value.TryGetValue(unitProfileIdHolder.Value, out var unitReactionConfigs))
                    throw new KeyNotFoundException($"{nameof(UnitReactionConfigsMap)} does not contains key: {unitProfileIdHolder.Value}");

                bool idleTimeExceeded = idleTimerSeconds.Value >= unitReactionConfigs.UnitIdleMaxDuration;

                reactionCanUpdateTag.ValueRW = isAliveTag.ValueRO && !workCanUpdateTag.ValueRO && idleTimeExceeded;
            }

        }

    }

}
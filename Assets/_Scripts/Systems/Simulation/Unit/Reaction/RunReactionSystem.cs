using Unity.Entities;
using Unity.Burst;
using Components;
using Components.Damage;
using Components.Unit;
using Components.Misc.GlobalConfigs;
using Components.Unit.Reaction;
using Utilities.Extensions;
using System.Collections.Generic;

namespace Systems.Simulation.Unit.Reaction
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [BurstCompile]
    public partial struct RunReactionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameGlobalConfigsICD>();

            EntityQuery query = SystemAPI.QueryBuilder()
                .WithAll<
                    RunStartedTag
                    , IsAliveTag
                    , IsUnitWorkingTag
                    , CanMoveEntityTag
                    , MoveSpeedLinear
                    , AnimatorData>()
                .Build();

            state.RequireForUpdate(query);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var unitReactionConfigsMap = SystemAPI.GetSingleton<UnitReactionConfigsMap>().Value;

            foreach (var (unitProfileIdHolderRef, reactionStartedTag, isAliveTag, isUnitWorkingTag, canMoveEntityTag, moveSpeedLinearRef, animatorDataRef) in
                SystemAPI.Query<
                    RefRO<UnitProfileIdHolder>
                    , EnabledRefRW <RunStartedTag>
                    , EnabledRefRO<IsAliveTag>
                    , EnabledRefRO<IsUnitWorkingTag>
                    , EnabledRefRO<CanMoveEntityTag>
                    , RefRO<MoveSpeedLinear>
                    , RefRW<AnimatorData>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!unitReactionConfigsMap.TryGetValue(unitProfileIdHolderRef.ValueRO.Value, out var unitReactionConfigs))
                    throw new KeyNotFoundException($"{nameof(UnitReactionConfigsMap)} does not contains key: {unitProfileIdHolderRef.ValueRO.Value}");

                bool currentSpeedIsRunSpeed = moveSpeedLinearRef.ValueRO.Value == unitReactionConfigs.UnitRunSpeed;
                bool canUpdate = isAliveTag.ValueRO && !isUnitWorkingTag.ValueRO && canMoveEntityTag.ValueRO && currentSpeedIsRunSpeed;
                bool reactionStarted = reactionStartedTag.ValueRO;

                if (canUpdate)
                {
                    if (!reactionStarted)
                    {
                        //OnReactionStart();
                        animatorDataRef.ValueRW.Value.ChangeValue("Running_A");
                        reactionStartedTag.ValueRW = true;
                    }

                    //OnReactionUpdate();

                }

                if (!canUpdate && reactionStarted)
                {
                    //OnReactionEnd();
                    reactionStartedTag.ValueRW = false;
                }

            }

        }

    }

}
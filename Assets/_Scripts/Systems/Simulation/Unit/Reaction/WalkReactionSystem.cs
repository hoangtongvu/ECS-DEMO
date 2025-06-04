using Unity.Entities;
using Unity.Burst;
using Components;
using Components.Unit;
using Components.Unit.Reaction;
using Utilities.Extensions;
using System.Collections.Generic;
using Components.GameEntity.Movement;
using Components.GameEntity.Damage;

namespace Systems.Simulation.Unit.Reaction
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [BurstCompile]
    public partial struct WalkReactionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQuery query = SystemAPI.QueryBuilder()
                .WithAll<
                    WalkStartedTag
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

            foreach (var (walkStartedTag, isAliveTag, profileIdHolderRef, isUnitWorkingTag, canMoveEntityTag, moveSpeedLinearRef, animatorDataRef) in
                SystemAPI.Query<
                    EnabledRefRW <WalkStartedTag>
                    , EnabledRefRO<IsAliveTag>
                    , RefRO<UnitProfileIdHolder>
                    , EnabledRefRO<IsUnitWorkingTag>
                    , EnabledRefRO<CanMoveEntityTag>
                    , RefRO<MoveSpeedLinear>
                    , RefRW<AnimatorData>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!unitReactionConfigsMap.TryGetValue(profileIdHolderRef.ValueRO.Value, out var unitReactionConfigs))
                    throw new KeyNotFoundException($"{nameof(UnitReactionConfigsMap)} does not contains key: {profileIdHolderRef.ValueRO.Value}");

                bool currentSpeedIsWalkSpeed = moveSpeedLinearRef.ValueRO.Value == unitReactionConfigs.UnitWalkSpeed;
                bool canUpdate = isAliveTag.ValueRO && !isUnitWorkingTag.ValueRO && canMoveEntityTag.ValueRO && currentSpeedIsWalkSpeed;
                bool reactionStarted = walkStartedTag.ValueRO;

                if (canUpdate)
                {
                    if (!reactionStarted)
                    {
                        //OnReactionStart();
                        animatorDataRef.ValueRW.Value.ChangeValue("Walking_A");
                        walkStartedTag.ValueRW = true;
                    }

                    //OnReactionUpdate();

                }

                if (!canUpdate && reactionStarted)
                {
                    //OnReactionEnd();
                    walkStartedTag.ValueRW = false;
                }

            }

        }

    }

}
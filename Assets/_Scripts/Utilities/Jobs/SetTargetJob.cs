using Components;
using Components.Unit.MyMoveCommand;
using Components.Unit.UnitSelection;
using Components.Unit;
using Core.Unit.MyMoveCommand;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Components.Misc.WorldMap.PathFinding;
using Components.GameEntity;
using Unity.Transforms;
using Utilities.Helpers.Misc;
using Core.Unit.Reaction;
using Core.Unit;
using System.Collections.Generic;
using Components.Unit.Misc;

namespace Utilities.Jobs
{
    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    [BurstCompile]
    public partial struct GetRunSpeedsJob : IJobEntity
    {
        [ReadOnly]
        public NativeHashMap<UnitProfileId, UnitReactionConfigs> UnitReactionConfigsMap;

        [NativeDisableParallelForRestriction]
        public NativeArray<float> OutputArray;

        [BurstCompile]
        void Execute(
            in UnitProfileIdHolder unitProfileIdHolder
            , in LocalTransform transform
            , ref AbsoluteDistanceXZToTarget absoluteDistanceXZToTarget
            , EnabledRefRO<UnitSelectedTag> unitSelectedTag
            , EnabledRefRW<CanFindPathTag> canFindPathTag
            , ref MoveSpeedLinear moveSpeedLinear
            , ref TargetEntity targetEntity
            , ref TargetEntityWorldSquareRadius worldSquareRadius
            , ref MoveCommandElement moveCommandElement
            , ref InteractingEntity interactingEntity
            , ref InteractionTypeICD interactionTypeICD
            , [EntityIndexInQuery] int entityIndex)
        {
            bool unitSelected = unitSelectedTag.ValueRO;
            if (!unitSelected) return;

            if (!this.UnitReactionConfigsMap.TryGetValue(unitProfileIdHolder.Value, out var unitReactionConfigs))
                throw new KeyNotFoundException($"{nameof(UnitReactionConfigsMap)} does not contains key: {unitProfileIdHolder.Value}");

            this.OutputArray[entityIndex] = unitReactionConfigs.UnitRunSpeed;

        }

    }

    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    [BurstCompile]
    public partial struct SetSingleTargetJobMultipleSpeeds : IJobEntity
    {
        [ReadOnly] public Entity TargetEntity;
        [ReadOnly] public half TargetEntityWorldSquareRadius;
        [ReadOnly] public float3 TargetPosition;
        [ReadOnly] public MoveCommandSource NewMoveCommandSource;
        [ReadOnly] public MoveCommandPrioritiesMap MoveCommandPrioritiesMap;

        [ReadOnly]
        public NativeArray<float> SpeedArray;

        [BurstCompile]
        void Execute(
            in ArmedStateHolder armedStateHolder
            , in LocalTransform transform
            , ref AbsoluteDistanceXZToTarget absoluteDistanceXZToTarget
            , EnabledRefRO<UnitSelectedTag> unitSelectedTag
            , EnabledRefRW<CanFindPathTag> canFindPathTag
            , ref MoveSpeedLinear moveSpeedLinear
            , ref TargetEntity targetEntity
            , ref TargetEntityWorldSquareRadius worldSquareRadius
            , ref MoveCommandElement moveCommandElement
            , ref InteractingEntity interactingEntity
            , ref InteractionTypeICD interactionTypeICD
            , [EntityIndexInQuery] int entityIndex)
        {
            bool unitSelected = unitSelectedTag.ValueRO;
            if (!unitSelected) return;

            bool canOverrideCommand =
                MoveCommandPrioritiesHelper.TryOverrideMoveCommand(
                    in this.MoveCommandPrioritiesMap
                    , ref moveCommandElement
                    , ref interactingEntity
                    , ref interactionTypeICD
                    , armedStateHolder.Value
                    , this.NewMoveCommandSource);

            if (!canOverrideCommand) return;

            moveCommandElement.TargetEntity = this.TargetEntity;
            moveCommandElement.Float3 = this.TargetPosition;

            targetEntity.Value = this.TargetEntity;
            worldSquareRadius.Value = this.TargetEntityWorldSquareRadius;

            moveSpeedLinear.Value = this.SpeedArray[entityIndex];
            canFindPathTag.ValueRW = true;

            AbsoluteDistanceXZToTargetHelper.SetDistance(
                ref absoluteDistanceXZToTarget
                , in transform.Position
                , this.TargetPosition);

        }

    }

}
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
    public partial struct Set_CanSetTargetJobScheduleTag_OnUnitSelected : IJobEntity
    {
        [BurstCompile]
        void Execute(
            EnabledRefRO<UnitSelectedTag> unitSelectedTag
            , EnabledRefRW<CanSetTargetJobScheduleTag> canSetTargetJobScheduleTag)
        {
            canSetTargetJobScheduleTag.ValueRW = unitSelectedTag.ValueRO;
        }

    }

    [WithAll(typeof(CanSetTargetJobScheduleTag))]
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
            , [EntityIndexInQuery] int entityIndex)
        {
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
            , EnabledRefRO<CanSetTargetJobScheduleTag> canSetTargetJobScheduleTag
            , EnabledRefRW<CanFindPathTag> canFindPathTag
            , ref MoveSpeedLinear moveSpeedLinear
            , ref TargetEntity targetEntity
            , ref TargetEntityWorldSquareRadius worldSquareRadius
            , ref MoveCommandElement moveCommandElement
            , ref InteractingEntity interactingEntity
            , ref InteractionTypeICD interactionTypeICD
            , ref InteractableDistanceRange interactableDistanceRange
            , [EntityIndexInQuery] int entityIndex)
        {
            if (!canSetTargetJobScheduleTag.ValueRO) return;

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

            interactableDistanceRange = InteractableDistanceRange.Default;

        }

    }

    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    [BurstCompile]
    public partial struct SetMultipleTargetsJobMultipleSpeeds : IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> TargetEntities;
        [ReadOnly] public NativeArray<float3> TargetPositions;
        [ReadOnly] public half TargetEntityWorldSquareRadius;// TODO: This must be a NArray
        [ReadOnly] public MoveCommandSource NewMoveCommandSource;
        [ReadOnly] public MoveCommandPrioritiesMap MoveCommandPrioritiesMap;

        [ReadOnly]
        public NativeArray<float> SpeedArray;

        [BurstCompile]
        void Execute(
            in ArmedStateHolder armedStateHolder
            , in LocalTransform transform
            , ref AbsoluteDistanceXZToTarget absoluteDistanceXZToTarget
            , EnabledRefRO<CanSetTargetJobScheduleTag> canSetTargetJobScheduleTag
            , EnabledRefRW<CanFindPathTag> canFindPathTag
            , ref MoveSpeedLinear moveSpeedLinear
            , ref TargetEntity targetEntity
            , ref TargetEntityWorldSquareRadius worldSquareRadius
            , ref MoveCommandElement moveCommandElement
            , ref InteractingEntity interactingEntity
            , ref InteractionTypeICD interactionTypeICD
            , ref InteractableDistanceRange interactableDistanceRange
            , [EntityIndexInQuery] int entityIndex)
        {
            if (!canSetTargetJobScheduleTag.ValueRO) return;

            bool canOverrideCommand =
                MoveCommandPrioritiesHelper.TryOverrideMoveCommand(
                    in this.MoveCommandPrioritiesMap
                    , ref moveCommandElement
                    , ref interactingEntity
                    , ref interactionTypeICD
                    , armedStateHolder.Value
                    , this.NewMoveCommandSource);

            if (!canOverrideCommand) return;

            moveCommandElement.TargetEntity = this.TargetEntities[entityIndex];
            moveCommandElement.Float3 = this.TargetPositions[entityIndex];

            targetEntity.Value = moveCommandElement.TargetEntity;
            worldSquareRadius.Value = this.TargetEntityWorldSquareRadius;

            moveSpeedLinear.Value = this.SpeedArray[entityIndex];
            canFindPathTag.ValueRW = true;

            AbsoluteDistanceXZToTargetHelper.SetDistance(
                ref absoluteDistanceXZToTarget
                , in transform.Position
                , moveCommandElement.Float3);

            interactableDistanceRange = InteractableDistanceRange.Default;

        }

    }

    [BurstCompile]
    public partial struct CleanUpCanSetTargetJobScheduleTagJob : IJobEntity
    {
        [BurstCompile]
        void Execute(
            EnabledRefRW<CanSetTargetJobScheduleTag> canSetTargetJobScheduleTag)
        {
            canSetTargetJobScheduleTag.ValueRW = false;
        }

    }

}
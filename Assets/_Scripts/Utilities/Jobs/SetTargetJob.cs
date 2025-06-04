using Components.Unit.UnitSelection;
using Components.Unit;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Components.Misc.WorldMap.PathFinding;
using Components.GameEntity;
using Unity.Transforms;
using Core.Unit.Reaction;
using Core.Unit;
using System.Collections.Generic;
using Components.Unit.Misc;
using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Components.GameEntity.Interaction;
using Utilities.Helpers.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;
using Core.GameEntity.Movement.MoveCommand;
using Utilities.Helpers.GameEntity.Movement.MoveCommand;

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
    public partial struct SetCanOverrideMoveCommandTagJob : IJobEntity
    {
        [ReadOnly] public MoveCommandPrioritiesMap MoveCommandPrioritiesMap;
        [ReadOnly] public MoveCommandSource NewMoveCommandSource;

        [BurstCompile]
        void Execute(
            EnabledRefRW<CanOverrideMoveCommandTag> canOverrideMoveCommandTag
            , in ArmedStateHolder armedStateHolder
            , ref MoveCommandElement moveCommandElement
            , ref InteractingEntity interactingEntity
            , ref InteractionTypeICD interactionTypeICD)
        {
            canOverrideMoveCommandTag.ValueRW =
                MoveCommandPrioritiesHelper.TryOverrideMoveCommand(
                    in this.MoveCommandPrioritiesMap
                    , ref moveCommandElement
                    , ref interactingEntity
                    , ref interactionTypeICD
                    , armedStateHolder.Value
                    , this.NewMoveCommandSource);

        }

    }

    [WithAll(typeof(CanSetTargetJobScheduleTag))]
    [WithAll(typeof(CanOverrideMoveCommandTag))]
    [BurstCompile]
    public partial struct SetSpeedsAsRunSpeedsJob : IJobEntity
    {
        [ReadOnly]
        public NativeHashMap<UnitProfileId, UnitReactionConfigs> UnitReactionConfigsMap;

        [BurstCompile]
        void Execute(
            in UnitProfileIdHolder unitProfileIdHolder
            , ref MoveSpeedLinear moveSpeedLinear)
        {
            if (!this.UnitReactionConfigsMap.TryGetValue(unitProfileIdHolder.Value, out var unitReactionConfigs))
                throw new KeyNotFoundException($"{nameof(UnitReactionConfigsMap)} does not contains key: {unitProfileIdHolder.Value}");

            moveSpeedLinear.Value = unitReactionConfigs.UnitRunSpeed;
        }

    }

    [WithAll(typeof(CanSetTargetJobScheduleTag))]
    [WithAll(typeof(CanOverrideMoveCommandTag))]
    [BurstCompile]
    public partial struct SetSingleTargetJobMultipleSpeeds : IJobEntity
    {
        [ReadOnly] public Entity TargetEntity;
        [ReadOnly] public half TargetEntityWorldSquareRadius;
        [ReadOnly] public float3 TargetPosition;

        [BurstCompile]
        void Execute(
            in LocalTransform transform
            , ref AbsoluteDistanceXZToTarget absoluteDistanceXZToTarget
            , EnabledRefRW<CanFindPathTag> canFindPathTag
            , ref TargetEntity targetEntity
            , ref TargetEntityWorldSquareRadius worldSquareRadius
            , ref MoveCommandElement moveCommandElement
            , ref InteractableDistanceRange interactableDistanceRange)
        {
            moveCommandElement.TargetEntity = this.TargetEntity;
            moveCommandElement.Float3 = this.TargetPosition;

            targetEntity.Value = this.TargetEntity;
            worldSquareRadius.Value = this.TargetEntityWorldSquareRadius;

            canFindPathTag.ValueRW = true;

            AbsoluteDistanceXZToTargetHelper.SetDistance(
                ref absoluteDistanceXZToTarget
                , in transform.Position
                , this.TargetPosition);

            interactableDistanceRange = InteractableDistanceRange.Default;

        }

    }

    [WithAll(typeof(CanSetTargetJobScheduleTag))]
    [WithAll(typeof(CanOverrideMoveCommandTag))]
    [BurstCompile]
    public partial struct SetMultipleTargetsJobMultipleSpeeds : IJobEntity
    {
        [ReadOnly] public NativeHashMap<Entity, TargetEntityInfo> MainEntityAndTargetInfoMap;
        [ReadOnly] public half TargetEntityWorldSquareRadius;// TODO: This must be a NArray

        [BurstCompile]
        void Execute(
            in LocalTransform transform
            , ref AbsoluteDistanceXZToTarget absoluteDistanceXZToTarget
            , EnabledRefRW<CanFindPathTag> canFindPathTag
            , ref TargetEntity targetEntity
            , ref TargetEntityWorldSquareRadius worldSquareRadius
            , ref MoveCommandElement moveCommandElement
            , ref InteractableDistanceRange interactableDistanceRange
            , Entity entity)
        {
            if (!this.MainEntityAndTargetInfoMap.TryGetValue(entity, out var targetEntityInfo))
            {
                UnityEngine.Debug.Log($"count: {this.MainEntityAndTargetInfoMap.Count}");
                foreach (var item in this.MainEntityAndTargetInfoMap)
                {
                    UnityEngine.Debug.Log($"key: {item.Key}");
                }
                throw new KeyNotFoundException($"{nameof(MainEntityAndTargetInfoMap)} does not contain key: {entity}");
            }

            moveCommandElement.TargetEntity = targetEntityInfo.TargetEntity;
            moveCommandElement.Float3 = targetEntityInfo.Position;

            targetEntity.Value = moveCommandElement.TargetEntity;
            worldSquareRadius.Value = this.TargetEntityWorldSquareRadius;

            canFindPathTag.ValueRW = true;

            AbsoluteDistanceXZToTargetHelper.SetDistance(
                ref absoluteDistanceXZToTarget
                , in transform.Position
                , moveCommandElement.Float3);

            interactableDistanceRange = InteractableDistanceRange.Default;

        }

    }

    [System.Serializable]
    public struct TargetEntityInfo
    {
        public Entity TargetEntity;
        public float3 Position;
    }

    [WithAll(typeof(CanSetTargetJobScheduleTag))]
    [WithAll(typeof(CanOverrideMoveCommandTag))]
    [BurstCompile]
    public partial struct SetTargetPositionsJob : IJobEntity
    {
        [ReadOnly] public NativeArray<float3> TargetPositions;

        [BurstCompile]
        void Execute(
            in LocalTransform transform
            , ref AbsoluteDistanceXZToTarget absoluteDistanceXZToTarget
            , EnabledRefRW<CanFindPathTag> canFindPathTag
            , ref TargetEntity targetEntity
            , ref TargetEntityWorldSquareRadius worldSquareRadius
            , ref MoveCommandElement moveCommandElement
            , ref InteractableDistanceRange interactableDistanceRange
            , [EntityIndexInQuery] int entityIndex)
        {
            moveCommandElement.TargetEntity = Entity.Null;
            moveCommandElement.Float3 = this.TargetPositions[entityIndex];

            targetEntity.Value = Entity.Null;
            worldSquareRadius.Value = half.zero;

            canFindPathTag.ValueRW = true;

            AbsoluteDistanceXZToTargetHelper.SetDistance(
                ref absoluteDistanceXZToTarget
                , in transform.Position
                , moveCommandElement.Float3);

            interactableDistanceRange = InteractableDistanceRange.Default;

        }

    }

    [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
    [BurstCompile]
    public partial struct CleanTagsJob : IJobEntity
    {
        [BurstCompile]
        void Execute(
            EnabledRefRW<CanSetTargetJobScheduleTag> canSetTargetJobScheduleTag
            , EnabledRefRW<CanOverrideMoveCommandTag> canOverrideMoveCommandTag)
        {
            canSetTargetJobScheduleTag.ValueRW = false;
            canOverrideMoveCommandTag.ValueRW = false;
        }

    }

}
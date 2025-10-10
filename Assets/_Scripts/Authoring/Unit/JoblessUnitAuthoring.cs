using Authoring.Utilities.Extensions;
using Authoring.Utilities.Helpers.GameEntity.InteractableActions;
using Components.GameEntity;
using Components.GameEntity.Damage;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Interaction;
using Components.GameEntity.Interaction.InteractionPhases;
using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;
using Components.GameEntity.Reaction;
using Components.GameResource;
using Components.GameResource.ItemPicking.Picker;
using Components.Misc;
using Components.Misc.Presenter;
using Components.Misc.WorldMap.PathFinding;
using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.Reaction;
using Components.Unit.UnitFeeding;
using Components.Unit.UnitSelection;
using Core.GameEntity;
using Core.GameEntity.Misc;
using Core.GameEntity.Movement.MoveCommand;
using Core.Unit.UnitFeeding;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Utilities.Helpers;

namespace Authoring.Unit
{
    public class JoblessUnitAuthoring : MonoBehaviour
    {
        private class Baker : Baker<JoblessUnitAuthoring>
        {
            public override void Bake(JoblessUnitAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<UnitTag>(entity);
                AddComponent<JoblessUnitTag>(entity);
                AddComponent<NewlySpawnedTag>(entity);
                AddComponent<NeedSpawnPresenterTag>(entity);
                this.AddAndDisableComponent<TakeHitEvent>(entity);
                this.AddAndDisableComponent<DeadEvent>(entity);

                AddComponent<HungerBarValue>(entity);
                AddComponent(entity, new CurrentHungerThreshold(HungerThreshold.Full));
                AddComponent<HungerThresholdChanged>(entity);

                IdleReaction.BakingHelper.BakeTags(this, in entity);
                AddComponent<IdleReaction.TimerSeconds>(entity);

                WalkReaction.BakingHelper.BakeTags(this, in entity);

                RunReaction.BakingHelper.BakeTags(this, in entity);

                InteractReaction.BakingHelper.BakeTags(this, in entity);

                HarvestReaction.BakingHelper.BakeTags(this, in entity);

                AttackReaction.BakingHelper.BakeTags(this, in entity);

                ConstructBuildingReaction.BakingHelper.BakeTags(this, in entity);

                PatrolReaction.BakingHelper.BakeTags(this, in entity);

                AddComponent<SelectableUnitTag>(entity);
                this.AddAndDisableComponent<UnitSelectedTag>(entity);

                AddComponent(entity, LookDirectionXZ.DefaultValue);
                AddComponent(entity, MoveDirectionFloat2.DefaultValue);
                AddComponent(entity, new MoveSpeedLinear
                {
                    Value = 0f,
                });
                AddComponent<MoveableEntityTag>(entity);
                this.AddAndDisableComponent<CanMoveEntityTag>(entity);

                AddComponent<CurrentWorldWaypoint>(entity);
                this.AddAndDisableComponent<TargetPosChangedTag>(entity);
                AddComponent<DistanceToCurrentWaypoint>(entity);
                AddComponent<TargetPosForPathFinding>(entity);
                AddComponent(entity, AbsoluteDistanceXZToTarget.MaxDistance);
                AddComponent(entity, new MoveCommandElement
                {
                    CommandSource = MoveCommandSource.None,
                    Float3 = float3.zero,
                    TargetEntity = Entity.Null,
                });

                AddComponent(entity, new UnitToolHolder
                {
                    Value = Entity.Null,
                });
                AddComponent<ToolProfileIdHolder>(entity);


                AddComponent<RotationFreezer>(entity);


                this.AddAndDisableComponent<CanInteractEntityTag>(entity);
                AddComponent(entity, new TargetEntity
                {
                    Value = Entity.Null,
                });
                AddComponent(entity, new TargetEntityWorldSquareRadius
                {
                    Value = half.zero,
                });
                AddComponent(entity, new InteractingEntity
                {
                    Value = Entity.Null,
                });
                AddComponent(entity, new InteractionTypeICD
                {
                    Value = InteractionType.None,
                });


                this.AddAndDisableComponent<NewlySelectedUnitTag>(entity);
                this.AddAndDisableComponent<NewlyDeselectedUnitTag>(entity);


                AddComponent<TargetPosMarkerHolder>(entity);
                AddComponent<SelectedUnitMarkerHolder>(entity);

                AddComponent(entity, new BaseDmg
                {
                    Value = 1,
                });
                AddComponent(entity, new BaseWorkSpeed
                {
                    Value = 1f,
                });

                AddComponent(entity, new WorkTimeCounterSecond
                {
                    Value = 0f,
                });

                ResourceWalletHelper.AddResourceWalletToEntity(this, entity);
                this.AddAndDisableComponent<WalletChangedTag>(entity);

                AddComponent(entity, new AnimatorData
                {
                    Value = new()
                    {
                        Value = "",
                        ValueChanged = true,
                    }
                });
                AddComponent(entity, new AnimatorTransitionDuration
                {
                    Value = 0.2f, // Default value for now.
                });

                AddBuffer<WaypointElement>(entity);
                this.AddAndDisableComponent<CanFindPathTag>(entity);

                AddComponent(entity, new ArmedStateHolder
                {
                    Value = ArmedState.False,
                });

                this.AddAndDisableComponent<CanSetTargetJobScheduleTag>(entity);

                this.AddAndDisableComponent<CanOverrideMoveCommandTag>(entity);

                AddComponent<IsUnarmedEntityTag>(entity);

                AddComponent(entity, InteractableDistanceRange.Default);

                this.AddAndDisableComponent<CanCheckInteractionRepeatTag>(entity);

                AddComponent(entity, new FactionIndex { Value = 1 });

                AddComponent<PresenterHandSlotsHolder>(entity);

                AddComponent<SpawnerEntityHolder>(entity);

                InteractableActionsBakingHelper.AddComponents(this, entity);

                AddComponent<NeedInitPosAroundSpawnerTag>(entity);

                AddBuffer<CandidateItemDistanceHit>(entity);
                this.AddAndDisableComponent<CandidateItemDistanceHitBufferUpdated>(entity);
                AddBuffer<ItemCanBePickedUpIndex>(entity);

                PreInteractionPhase.BakingHelper.BakeTags(this, entity);
                InteractingPhase.BakingHelper.BakeTags(this, entity);
                PostInteractionPhase.BakingHelper.BakeTags(this, entity);

            }

        }

    }

}
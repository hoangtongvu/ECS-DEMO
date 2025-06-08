using Components.GameEntity;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Interaction;
using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;
using Components.GameResource;
using Components.Misc;
using Components.Misc.Presenter;
using Components.Misc.WorldMap.PathFinding;
using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.Reaction;
using Components.Unit.UnitSelection;
using Core.GameEntity;
using Core.GameEntity.Misc;
using Core.GameEntity.Movement.MoveCommand;
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

                AddComponent<JoblessUnitTag>(entity);
                AddComponent<NewlySpawnedTag>(entity);
                AddComponent<ItemPickerTag>(entity);
                AddComponent<NeedSpawnPresenterTag>(entity);
                AddComponent<IdleStartedTag>(entity);
                SetComponentEnabled<IdleStartedTag>(entity, false);
                AddComponent<WorkStartedTag>(entity);
                SetComponentEnabled<WorkStartedTag>(entity, false);
                AddComponent<WalkStartedTag>(entity);
                SetComponentEnabled<WalkStartedTag>(entity, false);
                AddComponent<RunStartedTag>(entity);
                SetComponentEnabled<RunStartedTag>(entity, false);

                AddComponent<SelectableUnitTag>(entity);
                AddComponent<UnitSelectedTag>(entity);
                SetComponentEnabled<UnitSelectedTag>(entity, false);

                AddComponent(entity, MoveDirectionFloat2.DefaultValue);
                AddComponent(entity, new MoveSpeedLinear
                {
                    Value = 0f,
                });
                AddComponent<MoveableEntityTag>(entity);
                AddComponent<CanMoveEntityTag>(entity);
                SetComponentEnabled<CanMoveEntityTag>(entity, false);
                

                AddComponent<CurrentWorldWaypoint>(entity);
                AddComponent<TargetPosChangedTag>(entity);
                SetComponentEnabled<TargetPosChangedTag>(entity, false);
                AddComponent<DistanceToCurrentWaypoint>(entity);
                AddComponent(entity, AbsoluteDistanceXZToTarget.MaxDistance);
                AddComponent(entity, new MoveCommandElement
                {
                    CommandSource = MoveCommandSource.None,
                    Float3 = float3.zero,
                    TargetEntity = Entity.Null,
                });

                AddComponent(entity, new UnitIdleTimeCounter
                {
                    Value = 0,
                });

                AddComponent<NeedInitWalkTag>(entity);
                SetComponentEnabled<NeedInitWalkTag>(entity, false);


                AddComponent(entity, new UnitToolHolder
                {
                    Value = Entity.Null,
                });
                AddComponent<ToolProfileIdHolder>(entity);


                AddComponent<RotationFreezer>(entity);


                AddComponent<CanInteractEntityTag>(entity);
                SetComponentEnabled<CanInteractEntityTag>(entity, false);
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


                AddComponent<NewlySelectedUnitTag>(entity);
                SetComponentEnabled<NewlySelectedUnitTag>(entity, false);
                AddComponent<NewlyDeselectedUnitTag>(entity);
                SetComponentEnabled<NewlyDeselectedUnitTag>(entity, false);


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

                AddComponent<IsUnitWorkingTag>(entity);
                SetComponentEnabled<IsUnitWorkingTag>(entity, false);

                ResourceWalletHelper.AddResourceWalletToEntity(this, entity);
                AddComponent<WalletChangedTag>(entity);
                SetComponentEnabled<WalletChangedTag>(entity, false);

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
                AddComponent<CanFindPathTag>(entity);
                SetComponentEnabled<CanFindPathTag>(entity, false);

                AddComponent(entity, new ArmedStateHolder
                {
                    Value = ArmedState.False,
                });

                AddComponent<CanSetTargetJobScheduleTag>(entity);
                SetComponentEnabled<CanSetTargetJobScheduleTag>(entity, false);

                AddComponent<CanOverrideMoveCommandTag>(entity);
                SetComponentEnabled<CanOverrideMoveCommandTag>(entity, false);

                AddComponent<IsUnarmedEntityTag>(entity);

                AddComponent(entity, InteractableDistanceRange.Default);

                AddComponent<CanCheckInteractionRepeatTag>(entity);
                SetComponentEnabled<CanCheckInteractionRepeatTag>(entity, false);

                AddComponent(entity, new FactionIndex { Value = 1 });

                AddComponent<PresenterHandSlotsHolder>(entity);

            }

        }

    }

}
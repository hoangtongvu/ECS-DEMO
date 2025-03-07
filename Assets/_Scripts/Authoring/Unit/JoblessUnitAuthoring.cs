using Components;
using Components.Damage;
using Components.GameResource;
using Components.Misc;
using Components.Misc.Presenter;
using Components.Misc.WorldMap.PathFinding;
using Components.MyEntity;
using Components.MyEntity.EntitySpawning;
using Components.Tool;
using Components.Unit;
using Components.Unit.MyMoveCommand;
using Components.Unit.Reaction;
using Components.Unit.UnitSelection;
using Core.Misc.Presenter;
using Core.MyEntity;
using Core.Unit;
using Core.Unit.MyMoveCommand;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Utilities.Helpers;

namespace Authoring.Unit
{
    public class JoblessUnitAuthoring : MonoBehaviour
    {
        [SerializeField] private UnitType unitType; //Can put this into Unit profile SO;
        [SerializeField] private PresenterPrefabId presenterPrefabId; //Can put this into Unit profile SO;
        [SerializeField] private ushort localIndex; //Can put this into Unit profile SO;
        [SerializeField] private int maxHp = 100; //Can put this into Unit profile SO;
        [SerializeField] private int currentHp = 100; //Can put this into Unit profile SO;
        [SerializeField] private float speed = 5f; //Can put this into Unit profile SO;

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

                AddComponent(entity, new PresenterPrefabIdHolder
                {
                    Value = authoring.presenterPrefabId,
                });
                AddComponent<PresenterHolder>(entity);

                AddComponent(entity, new UnitId
                {
                    UnitType = authoring.unitType,
                    LocalIndex = authoring.localIndex,
                });

                AddComponent<SelectableUnitTag>(entity);
                AddComponent<UnitSelectedTag>(entity);
                SetComponentEnabled<UnitSelectedTag>(entity, false);


                AddComponent(entity, new HpComponent
                {
                    CurrentHp = authoring.currentHp,
                    MaxHp = authoring.maxHp,
                });
                AddComponent<HpChangedTag>(entity);
                SetComponentEnabled<HpChangedTag>(entity, false);
                AddComponent<HpChangedValue>(entity);
                AddComponent<IsAliveTag>(entity);


                AddComponent<MoveDirectionFloat2>(entity);
                AddComponent(entity, new MoveSpeedLinear
                {
                    Value = authoring.speed,
                });
                AddComponent<MoveableEntityTag>(entity);
                AddComponent<CanMoveEntityTag>(entity);
                SetComponentEnabled<CanMoveEntityTag>(entity, false);
                

                AddComponent<TargetPosition>(entity);
                AddComponent<TargetPosChangedTag>(entity);
                SetComponentEnabled<TargetPosChangedTag>(entity, false);
                AddComponent(entity, new DistanceToTarget
                {
                    MinDistance = 1f, //TODO: Find another way to get this value;
                });
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
                AddComponent<ToolTypeICD>(entity);


                AddComponent<RotationFreezer>(entity);


                AddComponent<CanInteractEntityTag>(entity);
                SetComponentEnabled<CanInteractEntityTag>(entity, false);
                AddComponent(entity, new TargetEntity
                {
                    Value = Entity.Null,
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

            }
        }
    }
}

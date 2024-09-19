using Components;
using Components.Damage;
using Components.MyEntity;
using Components.MyEntity.EntitySpawning;
using Components.Unit;
using Components.Unit.MyMoveCommand;
using Components.Unit.UnitSelection;
using Core.MyEntity;
using Core.Unit;
using Core.Unit.MyMoveCommand;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring.Unit
{
    public class JoblessUnitAuthoring : MonoBehaviour
    {
        [SerializeField] private UnitType unitType; //Can put this into Unit profile SO;
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

                AddComponent<NeedsInitWalkTag>(entity);
                SetComponentEnabled<NeedsInitWalkTag>(entity, false);


                AddComponent(entity, new UnitToolHolder
                {
                    Value = Entity.Null,
                });


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


                AddComponent<UnitTargetPosUIID>(entity);

            }
        }
    }
}

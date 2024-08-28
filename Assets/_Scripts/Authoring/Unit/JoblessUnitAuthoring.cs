using Components;
using Components.Damage;
using Components.MyEntity;
using Components.MyEntity.EntitySpawning;
using Components.Unit;
using Core.Unit;
using Unity.Entities;
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
        [SerializeField] private bool moveableState;

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
                AddComponent(entity, new UnitSelected
                {
                    Value = false,
                });


                AddComponent(entity, new HpComponent
                {
                    CurrentHp = authoring.currentHp,
                    MaxHp = authoring.maxHp,
                });
                AddComponent(entity, new HpChangeState
                {
                    IsChanged = false,
                    ChangedValue = 0,
                });
                AddComponent(entity, new AliveState
                {
                    Value = true,
                });


                AddComponent<MoveDirectionFloat2>(entity);
                AddComponent(entity, new MoveSpeedLinear
                {
                    Value = authoring.speed,
                });
                AddComponent(entity, new MoveableState
                {
                    Entity = entity,
                });
                SetComponentEnabled<MoveableState>(entity, authoring.moveableState);


                AddComponent<TargetPosition>(entity);
                AddComponent(entity, new DistanceToTarget
                {
                    MinDistance = 1f, //TODO: Find another way to get this value;
                });
                AddComponent(entity, new MoveAffecterICD
                {
                    Value = Core.Unit.MoveAffecter.None,
                });


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

            }
        }
    }
}

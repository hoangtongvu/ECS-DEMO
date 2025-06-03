using Components.Damage;
using Components.GameEntity;
using Components.GameEntity.Misc;
using Components.Misc;
using Components.Misc.Presenter;
using Components.Misc.Presenter.PresenterPrefabGO;
using Components.Player;
using Components.Unit.NearUnitDropItems;
using Core.GameEntity.Misc;
using Core.Misc.Presenter.PresenterPrefabGO;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Player
{
    public class PlayerTagAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PlayerTagAuthoring>
        {
            public override void Bake(PlayerTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<PlayerTag>(entity);
                AddComponent<ItemPickerTag>(entity);

                AddBuffer<NearbyUnitDropItemTimerElement>(entity);

                AddComponent<NeedSpawnPresenterTag>(entity);
                AddComponent(entity, new PresenterPrefabGOKeyHolder
                {
                    Value = PresenterPrefabGOKey.Null,
                });

                AddComponent<InteractableEntityTag>(entity);
                AddComponent(entity, new ArmedStateHolder
                {
                    Value = ArmedState.True,
                });

                AddComponent(entity, new HpComponent
                {
                    CurrentHp = 100,
                    MaxHp = 100,
                });
                AddComponent<HpChangedTag>(entity);
                SetComponentEnabled<HpChangedTag>(entity, false);
                AddComponent<HpChangedValue>(entity);
                AddComponent<IsAliveTag>(entity);

                AddComponent(entity, new FactionIndex { Value = 1 });

            }

        }

    }

}

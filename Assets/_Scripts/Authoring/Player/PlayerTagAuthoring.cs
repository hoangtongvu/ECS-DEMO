using Components.GameEntity.Damage;
using Components.GameEntity.Interaction;
using Components.GameEntity.Misc;
using Components.Misc;
using Components.Misc.Presenter;
using Components.Player;
using Components.Unit.InteractableActions;
using Components.Unit.NearUnitDropItems;
using Core.GameEntity.Misc;
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

                AddComponent<InteractableEntityTag>(entity);
                AddComponent(entity, new ArmedStateHolder
                {
                    Value = ArmedState.True,
                });

                AddComponent(entity, new FactionIndex { Value = 1 });

                AddComponent(entity, new DmgValue
                {
                    Value = 10,
                });

                AddComponent<NearestInteractableEntity>(entity);

            }

        }

    }

}

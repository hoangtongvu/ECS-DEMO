using Components.Misc;
using Components.Misc.Presenter;
using Components.Player;
using Components.Unit.NearUnitDropItems;
using Core.Misc.Presenter;
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

                AddComponent(entity, new PresenterPrefabIdHolder
                {
                    Value = new()
                    {
                        PresenterType = PresenterType.Player,
                        LocalIndex = 0,
                    }
                });
                AddComponent<PresenterHolder>(entity);
                AddComponent<TransformAccessArrayIndex>(entity);

            }

        }

    }

}

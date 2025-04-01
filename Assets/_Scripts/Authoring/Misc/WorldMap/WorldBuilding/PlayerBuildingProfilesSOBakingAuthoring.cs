using Components.GameEntity;
using Components.Misc.WorldMap.WorldBuilding;
using Core.Misc.WorldMap.WorldBuilding;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.WorldMap.WorldBuilding
{
    public class PlayerBuildingProfilesSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private PlayerBuildingProfilesSO profilesSO;

        private class Baker : Baker<PlayerBuildingProfilesSOBakingAuthoring>
        {
            public override void Bake(PlayerBuildingProfilesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new PlayerBuildingProfilesSOHolder
                {
                    Value = authoring.profilesSO,
                });

                var buffer = AddBuffer<AfterBakedPrefabsElement>(entity);

                foreach (var profile in authoring.profilesSO.Profiles)
                {
                    var entityPrefabsElement = new AfterBakedPrefabsElement();

                    entityPrefabsElement.OriginalPresenterGO = profile.Value.PresenterPrefab;

                    if (profile.Value.IsPresenterEntity)
                    {
                        entityPrefabsElement.PresenterEntity = GetEntity(profile.Value.PresenterPrefab, TransformUsageFlags.None);
                    }

                    entityPrefabsElement.PrimaryEntity = GetEntity(profile.Value.PrimaryEntityPrefab, TransformUsageFlags.Dynamic);

                    buffer.Add(entityPrefabsElement);
                }
                
            }

        }

    }

}

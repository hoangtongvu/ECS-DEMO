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

                var buffer = AddBuffer<BakedGameEntityProfileElement>(entity);

                foreach (var profile in authoring.profilesSO.Profiles)
                {
                    buffer.Add(new()
                    {
                        OriginalPresenterGO = profile.Value.PresenterPrefab,
                        PrimaryEntity = GetEntity(profile.Value.PrimaryEntityPrefab, TransformUsageFlags.Dynamic),
                        PresenterEntity = profile.Value.IsPresenterEntity
                            ? GetEntity(profile.Value.PresenterPrefab, TransformUsageFlags.None)
                            : Entity.Null,
                        GameEntitySize = profile.Value.GameEntitySize,
                    });

                }
                
            }

        }

    }

}

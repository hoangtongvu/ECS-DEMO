using Components.GameEntity;
using Components.GameResource;
using Core.GameResource;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameResource
{
    public class ResourceProfilesSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private ResourceProfilesSO profilesSO;

        private class Baker : Baker<ResourceProfilesSOBakingAuthoring>
        {
            public override void Bake(ResourceProfilesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new ResourceProfilesSOHolder
                {
                    Value = authoring.profilesSO,
                });

                var buffer = AddBuffer<AfterBakedPrefabsElement>(entity);

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

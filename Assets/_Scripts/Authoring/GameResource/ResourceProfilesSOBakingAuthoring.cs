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

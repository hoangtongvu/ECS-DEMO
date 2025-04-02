using Authoring.Utilities.Helpers.GameEntity;
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

                ProfilesSOBakingHelper.BakeGameEntityProfileElementBuffer(this, authoring.profilesSO, entity);

            }

        }

    }

}

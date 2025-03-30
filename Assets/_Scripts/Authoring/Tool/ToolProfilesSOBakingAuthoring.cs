using Components.GameEntity;
using Components.Tool;
using Core.Tool;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Tool
{
    public class ToolProfilesSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private ToolProfilesSO profilesSO;

        private class Baker : Baker<ToolProfilesSOBakingAuthoring>
        {
            public override void Bake(ToolProfilesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new ToolProfilesSOHolder
                {
                    Value = authoring.profilesSO,
                });

                var buffer = AddBuffer<AfterBakedPrefabsElement>(entity);

                foreach (var profile in authoring.profilesSO.Profiles)
                {
                    var entityPrefabsElement = new AfterBakedPrefabsElement();

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

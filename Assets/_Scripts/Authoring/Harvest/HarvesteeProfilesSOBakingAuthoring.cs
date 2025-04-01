using Components.GameEntity;
using Components.Harvest;
using Core.Harvest;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Harvest
{
    public class HarvesteeProfilesSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private HarvesteeProfilesSO profilesSO;

        private class Baker : Baker<HarvesteeProfilesSOBakingAuthoring>
        {
            public override void Bake(HarvesteeProfilesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new HarvesteeProfilesSOHolder
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

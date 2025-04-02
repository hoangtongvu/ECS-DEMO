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

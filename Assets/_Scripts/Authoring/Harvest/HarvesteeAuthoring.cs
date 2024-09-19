using Components.Harvest;
using Components.MyEntity;
using Components.MyEntity.EntitySpawning;
using Core.Harvest;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Harvest
{
    public class HarvesteeAuthoring : MonoBehaviour
    {
        [SerializeField] private HarvesteeProfileId harvesteeProfileId;

        private class Baker : Baker<HarvesteeAuthoring>
        {
            public override void Bake(HarvesteeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<NewlySpawnedTag>(entity);
                AddComponent<InteractableEntityTag>(entity);

                AddComponent<HarvesteeTag>(entity);
                AddComponent<HarvesteeHealthId>(entity);
                AddComponent<HarvesteeHealthChangedTag>(entity);

                AddComponent(entity, new HarvesteeProfileIdHolder
                {
                    Value = authoring.harvesteeProfileId,
                });

                AddComponent<DropResourceHpThreshold>(entity);
            }
        }
    }
}

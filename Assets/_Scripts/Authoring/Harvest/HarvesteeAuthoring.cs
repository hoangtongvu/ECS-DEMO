using Components.Harvest;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Harvest
{
    public class HarvesteeAuthoring : MonoBehaviour
    {

        private class Baker : Baker<HarvesteeAuthoring>
        {
            public override void Bake(HarvesteeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<HarvesteeTag>(entity);
                AddComponent<HarvesteeHealthId>(entity);
                AddComponent<HarvesteeHealthChangedTag>(entity);
            }
        }
    }
}

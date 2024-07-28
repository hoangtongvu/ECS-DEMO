using Components.Harvest;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Harvest
{
    public class HarvesteeTagAuthoring : MonoBehaviour
    {

        private class Baker : Baker<HarvesteeTagAuthoring>
        {
            public override void Bake(HarvesteeTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<HarvesteeTag>(entity);
                AddComponent<HarvesteeHealthId>(entity);
            }
        }
    }
}

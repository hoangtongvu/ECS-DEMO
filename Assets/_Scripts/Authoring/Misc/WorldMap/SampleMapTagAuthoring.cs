using Components.Misc.WorldMap;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.WorldMap
{
    public class SampleMapTagAuthoring : MonoBehaviour
    {
        private class Baker : Baker<SampleMapTagAuthoring>
        {
            public override void Bake(SampleMapTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<SampleMapTag>(entity);

            }
        }
    }
}

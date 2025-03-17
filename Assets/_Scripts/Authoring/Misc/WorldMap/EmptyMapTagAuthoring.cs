using Components.Misc.WorldMap;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.WorldMap
{
    public class EmptyMapTagAuthoring : MonoBehaviour
    {
        private class Baker : Baker<EmptyMapTagAuthoring>
        {
            public override void Bake(EmptyMapTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<EmptyMapTag>(entity);

            }
        }
    }
}

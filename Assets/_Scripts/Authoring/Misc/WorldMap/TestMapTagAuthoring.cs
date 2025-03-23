using Components.Misc.WorldMap;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.WorldMap
{
    public class TestMapTagAuthoring : MonoBehaviour
    {
        private class Baker : Baker<TestMapTagAuthoring>
        {
            public override void Bake(TestMapTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<TestMapTag>(entity);

            }
        }
    }
}

using Components.Tool;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Tool
{
    public class DerelictToolTagAuthoring : MonoBehaviour
    {
        private class Baker : Baker<DerelictToolTagAuthoring>
        {
            public override void Bake(DerelictToolTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<DerelictToolTag>(entity);
            }
        }

    }
}

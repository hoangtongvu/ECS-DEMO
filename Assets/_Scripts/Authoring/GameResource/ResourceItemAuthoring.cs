using Components.GameResource;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameResource
{

    public class ResourceItemAuthoring : MonoBehaviour
    {

        private class Baker : Baker<ResourceItemAuthoring>
        {
            public override void Bake(ResourceItemAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<ResourceItemICD>(entity);
            }
        }
    }
}

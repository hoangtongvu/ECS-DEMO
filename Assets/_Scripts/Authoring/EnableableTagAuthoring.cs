using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class EnableableTagAuthoring : MonoBehaviour
    {

        private class Baker : Baker<EnableableTagAuthoring>
        {
            public override void Bake(EnableableTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<EnableableTag>(entity);

            }
        }
    }
}

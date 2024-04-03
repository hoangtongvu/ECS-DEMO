using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class SelfEntityRefAuthoring : MonoBehaviour
    {

        private class Baker : Baker<SelfEntityRefAuthoring>
        {
            public override void Bake(SelfEntityRefAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new SelfEntityRef
                {
                    Value = entity,
                });

            }
        }
    }
}

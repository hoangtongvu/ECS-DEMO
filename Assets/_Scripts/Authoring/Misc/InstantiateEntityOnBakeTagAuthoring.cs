using Components.Misc;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc
{
    public class InstantiateEntityOnBakeTagAuthoring : MonoBehaviour
    {
        private class Baker : Baker<InstantiateEntityOnBakeTagAuthoring>
        {
            public override void Bake(InstantiateEntityOnBakeTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent<InstantiateEntityOnBakeTag>(entity);

            }

        }

    }

}

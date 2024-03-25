using Components.Tag;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class GroundTagAuthoring : MonoBehaviour
    {

        private class Baker : Baker<GroundTagAuthoring>
        {
            public override void Bake(GroundTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<GroundTag>(entity);

            }
        }
    }
}

using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class EnableableTagAuthoring : MonoBehaviour
    {

        private class Baker : Baker<MovementAuthoring>
        {
            public override void Bake(MovementAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<EnableableTag>(entity);

            }
        }
    }
}

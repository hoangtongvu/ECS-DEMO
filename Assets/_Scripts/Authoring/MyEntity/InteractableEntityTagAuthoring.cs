using Components.MyEntity;
using Unity.Entities;
using UnityEngine;

namespace Authoring.MyEntity
{
    public class InteractableEntityTagAuthoring : MonoBehaviour
    {

        private class Baker : Baker<InteractableEntityTagAuthoring>
        {
            public override void Bake(InteractableEntityTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<InteractableEntityTag>(entity);
            }
        }
    }
}

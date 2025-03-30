using Components.GameEntity;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameEntity
{
    public class CanInteractEntityAuthoring : MonoBehaviour
    {

        private class Baker : Baker<CanInteractEntityAuthoring>
        {
            public override void Bake(CanInteractEntityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<CanInteractEntityTag>(entity);
                SetComponentEnabled<CanInteractEntityTag>(entity, false);

                AddComponent(entity, new TargetEntity
                {
                    Value = Entity.Null,
                });
            }
        }
    }
}

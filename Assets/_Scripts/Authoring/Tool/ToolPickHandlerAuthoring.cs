using Components.Tool;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Tool
{
    public class ToolPickHandlerAuthoring : MonoBehaviour
    {
        private class Baker : Baker<ToolPickHandlerAuthoring>
        {
            public override void Bake(ToolPickHandlerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<CanBePicked>(entity);
                AddComponent(entity, new PickedBy
                {
                    Value = Entity.Null,
                });
            }
        }

    }
}

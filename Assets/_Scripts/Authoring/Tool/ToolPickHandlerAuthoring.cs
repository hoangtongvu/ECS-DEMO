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

                AddComponent<CanBePickedTag>(entity);
                SetComponentEnabled<CanBePickedTag>(entity, false);

                AddComponent(entity, new ToolPickerEntity
                {
                    Value = Entity.Null,
                });
            }
        }

    }
}

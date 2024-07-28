using Components.Tool;
using Core.Tool;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Tool
{
    public class ToolTypeICDAuthoring : MonoBehaviour
    {
        [SerializeField] private ToolType toolType;
        private class Baker : Baker<ToolTypeICDAuthoring>
        {
            public override void Bake(ToolTypeICDAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ToolTypeICD
                {
                    Value = authoring.toolType,
                });
            }
        }

    }
}

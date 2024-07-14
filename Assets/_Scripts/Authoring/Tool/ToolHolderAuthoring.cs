using Components.MyEntity.EntitySpawning;
using Components.Tool;
using Core.Tool;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Tool
{
    public class ToolHolderAuthoring : MonoBehaviour
    {
        private class Baker : Baker<ToolHolderAuthoring>
        {
            public override void Bake(ToolHolderAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<SpawnTypeTag<ToolType>>(entity);

                AddComponent(entity, new ToolHoldLimit
                {
                    Value = 3,
                });

                AddComponent<ToolHoldCount>(entity);

            }
        }

    }
}

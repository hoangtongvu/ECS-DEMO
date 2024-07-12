using Components.MyEntity.EntitySpawning;
using Components.Tool;
using Core.Tool;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Tool
{
    public class ToolHolderAuthoring : MonoBehaviour
    {
        [SerializeField] private float callerRadius = 15f;
        [SerializeField] private float pickUpToolRadius = 3f;
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

                AddBuffer<ToolHolderElement>(entity);

                AddComponent(entity, new ToolCallerRadius
                {
                    Value = authoring.callerRadius,
                });

                AddComponent(entity, new ToolPickRadius
                {
                    Value = authoring.pickUpToolRadius,
                });

            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, this.callerRadius);
            Gizmos.DrawWireSphere(transform.position, this.pickUpToolRadius);
        }
    }
}

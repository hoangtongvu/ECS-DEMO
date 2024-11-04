using Components.Misc.FlowField;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.FlowField
{
    public class GridDebugConfigAuthoring : MonoBehaviour
    {
        [SerializeField] private bool showCost = true;
        [SerializeField] private bool showBestCost = true;
        [SerializeField] private bool showDirectionVector = true;
        [SerializeField] private bool showGridLines = true;

        private class Baker : Baker<GridDebugConfigAuthoring>
        {
            public override void Bake(GridDebugConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new GridDebugConfig
                {
                    ShowCost = authoring.showCost,
                    ShowBestCost = authoring.showBestCost,
                    ShowDirectionVector = authoring.showDirectionVector,
                    ShowGridLines = authoring.showGridLines,
                });

            }
        }
    }
}

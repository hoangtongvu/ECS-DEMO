using Components.Misc.FlowField;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.FlowField
{
    public class GridNodePresenterConfigAuthoring : MonoBehaviour
    {
        [SerializeField] private bool showCost = true;
        [SerializeField] private bool showBestCost = true;
        [SerializeField] private bool showDirectionVector = true;
        [SerializeField] private bool showGridLines = true;

        private class Baker : Baker<GridNodePresenterConfigAuthoring>
        {
            public override void Bake(GridNodePresenterConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new GridNodePresenterConfig
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

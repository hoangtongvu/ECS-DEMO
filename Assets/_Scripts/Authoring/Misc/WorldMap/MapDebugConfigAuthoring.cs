using Components.Misc.WorldMap;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.WorldMap
{
    public class GridDebugConfigAuthoring : MonoBehaviour
    {
        [SerializeField] private bool showCellCosts = true;
        [SerializeField] private bool showCellGridLines = true;
        [SerializeField] private bool showChunks = true;

        private class Baker : Baker<GridDebugConfigAuthoring>
        {
            public override void Bake(GridDebugConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new MapDebugConfig
                {
                    ShowCellCosts = authoring.showCellCosts,
                    ShowCellGridLines = authoring.showCellGridLines,
                    ShowChunks = authoring.showChunks,
                });

            }
        }
    }
}

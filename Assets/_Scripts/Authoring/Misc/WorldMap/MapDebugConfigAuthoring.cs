using Components.Misc.WorldMap;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.WorldMap
{
    public class GridDebugConfigAuthoring : MonoBehaviour
    {
        [SerializeField] private bool showCost = true;
        [SerializeField] private bool showGridLines = true;

        private class Baker : Baker<GridDebugConfigAuthoring>
        {
            public override void Bake(GridDebugConfigAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new MapDebugConfig
                {
                    ShowCost = authoring.showCost,
                    ShowGridLines = authoring.showGridLines,
                });

            }
        }
    }
}

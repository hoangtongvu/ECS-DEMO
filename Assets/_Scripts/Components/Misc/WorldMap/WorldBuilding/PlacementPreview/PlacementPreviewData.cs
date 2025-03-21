using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap.WorldBuilding.PlacementPreview
{
    public struct PlacementPreviewData : IComponentData
    {
        public float3 TopLeftCellCenterPos;
        public float PlacementSpriteScale;
        public bool IsBuildable;
    }

}

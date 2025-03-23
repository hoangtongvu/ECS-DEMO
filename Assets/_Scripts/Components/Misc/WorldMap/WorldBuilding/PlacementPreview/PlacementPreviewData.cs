using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap.WorldBuilding.PlacementPreview
{
    public struct PlacementPreviewData : IComponentData
    {
        public int2 TopLeftCellGridPos;
        public float3 BuildingCenterPosOnGround;
        public float PlacementSpriteScale;
        public bool IsBuildable;
    }

}

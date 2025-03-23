using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap.WorldBuilding
{
    public struct BuildCommand
    {
        public Entity Entity;
        public int2 TopLeftCellGridPos;
        public float3 BuildingCenterPos;
        public int GridSquareSize;
    }

    public struct BuildCommandQueue : IComponentData
    {
        public NativeList<BuildCommand> Value;
    }

}

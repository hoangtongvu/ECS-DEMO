using Core.Misc.WorldMap;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap
{
    public struct WorldTileCostMap : IComponentData
    {
        public NativeArray<Cell> Value;
        public int Width;
        public int Height;
        public int2 Offset;
    }

}

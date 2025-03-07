using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap.PathFinding
{
    public struct WaypointElement : IBufferElementData
    {
        public int2 Value;
    }

}

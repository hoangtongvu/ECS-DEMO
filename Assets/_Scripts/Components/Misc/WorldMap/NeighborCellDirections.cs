using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap
{
    /// <summary>
    /// Directions' order:
    /// <code>
    ///  7  0  4
    ///  3  x  1
    ///  6  2  5
    /// </code>
    /// </summary>
    public struct NeighborCellDirections : IComponentData
    {
        public NativeArray<int2> Value;
    }

}

using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap.LineCaching
{
    public struct CellPositionsContainer : IComponentData
    {
        public NativeList<int2> Value;
    }

}

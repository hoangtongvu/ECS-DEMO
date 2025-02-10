using Core.Misc.WorldMap.LineCaching;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.WorldMap.LineCaching
{
    public struct CellPosRangeMap : IComponentData
    {
        public NativeHashMap<LineCacheKey, CellPosRange> Value;
    }

}

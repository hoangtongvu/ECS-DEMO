using Core.Misc.WorldMap.LineCaching;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap.LineCaching;

public struct CachedLines : IComponentData
{
    public NativeParallelMultiHashMap<LineCacheKey, int2> Value;
}
using Core.Misc.WorldMap.ChunkInnerPathCost;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.WorldMap.ChunkInnerPathCost
{
    public struct InnerPathCostMap : IComponentData
    {
        public NativeHashMap<InnerPathKey, float> Value;

    }

}

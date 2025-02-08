using Core.Misc.WorldMap;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.WorldMap
{
    public struct ChunkIndexToExitsMap : IComponentData
    {
        public NativeList<ChunkExitRange> Value;
    }

}

using Core.Misc.WorldMap;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.WorldMap
{
    public struct ChunkIndexToExitIndexesMap : IComponentData
    {
        public NativeList<ChunkExitIndexRange> Value;
    }

}

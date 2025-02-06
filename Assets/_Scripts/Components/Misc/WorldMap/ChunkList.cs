using Core.Misc.WorldMap;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.WorldMap
{
    public struct ChunkList : IComponentData
    {
        public NativeList<Chunk> Value;
    }

}

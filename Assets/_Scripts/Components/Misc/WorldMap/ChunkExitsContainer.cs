using Core.Misc.WorldMap;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.WorldMap
{
    public struct ChunkExitsContainer : IComponentData
    {
        public NativeList<ChunkExit> Value;
    }

}

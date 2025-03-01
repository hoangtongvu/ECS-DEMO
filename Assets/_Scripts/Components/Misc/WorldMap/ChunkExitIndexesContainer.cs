using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.WorldMap
{
    public struct ChunkExitIndexesContainer : IComponentData
    {
        public NativeList<int> Value;
    }

}

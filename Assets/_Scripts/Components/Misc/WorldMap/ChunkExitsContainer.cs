using Core.Misc.WorldMap;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.WorldMap
{
    /// <summary>
    /// Used to store non-duplicated exits in the whole map.
    /// </summary>
    public struct ChunkExitsContainer : IComponentData
    {
        public NativeList<ChunkExit> Value;
    }

}

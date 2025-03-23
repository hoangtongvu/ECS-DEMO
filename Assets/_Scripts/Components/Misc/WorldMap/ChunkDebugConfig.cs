using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Components.Misc.WorldMap
{
    public struct ChunkDebugConfig : IComponentData
    {
        public NativeArray<Color> GridLineColorPalette;
    }

}

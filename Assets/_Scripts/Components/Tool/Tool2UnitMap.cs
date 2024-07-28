using Unity.Collections;
using Unity.Entities;

namespace Components.Tool
{
    public struct Tool2UnitMap : IComponentData
    {
        public NativeHashMap<byte, byte> Value;
    }
}

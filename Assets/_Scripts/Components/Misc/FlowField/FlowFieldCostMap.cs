using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.FlowField
{
    public struct FlowFieldCostMap : IComponentData
    {
        public NativeArray<byte> Value;
        public int Width;
        public int Height;
        public int2 Offset;
    }

}

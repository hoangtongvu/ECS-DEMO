using System.Runtime.InteropServices;
using Unity.Mathematics;

namespace Components.Misc.Tween
{
    [StructLayout(LayoutKind.Explicit)]
    public struct UnionType
    {
        [FieldOffset(0)] public float Float;
        [FieldOffset(0)] public float2 Float2;
        [FieldOffset(0)] public float3 Float3;
        [FieldOffset(0)] public float4 Float4;
    }

}

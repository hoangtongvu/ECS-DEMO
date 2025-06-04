using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc
{
    public struct RotateDirection : IComponentData
    {
        public float3 value;
    }

    public struct RotateSpeed : IComponentData
    {
        public float value;
    }
}

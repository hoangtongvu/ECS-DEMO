using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct MoveDirection : IComponentData
    {
        public float3 value;
    }

    public struct MoveSpeed : IComponentData
    {
        public float value;
    }
}

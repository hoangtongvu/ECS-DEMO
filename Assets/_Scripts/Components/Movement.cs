using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct MoveDirectionFloat3 : IComponentData
    {
        public float3 Value;
    }

    public struct MoveDirectionFloat2 : IComponentData
    {
        public float2 Value;
        public static MoveDirectionFloat2 DefaultValue = new()
        {
            Value = new(0, 1),
        };
    }

    public struct MoveSpeedLinear : IComponentData
    {
        public float Value;
    }

    public struct CanMoveEntityTag : IComponentData, IEnableableComponent
    {
    }

    public struct MoveableEntityTag : IComponentData
    {
    }

}

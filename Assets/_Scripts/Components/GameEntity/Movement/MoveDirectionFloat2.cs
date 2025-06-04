using Unity.Entities;
using Unity.Mathematics;

namespace Components.GameEntity.Movement
{
    public struct MoveDirectionFloat2 : IComponentData
    {
        public float2 Value;
        public static MoveDirectionFloat2 DefaultValue = new()
        {
            Value = new(0, 1),
        };
    }
}

using Unity.Entities;
using Unity.Mathematics;

namespace Components.GameEntity.Misc
{
    public struct LookDirectionXZ : IComponentData
    {
        public float2 Value;
        public static LookDirectionXZ DefaultValue = new()
        {
            Value = new(0, 1),
        };
    }
}

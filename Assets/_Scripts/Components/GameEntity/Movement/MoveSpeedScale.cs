using Unity.Entities;

namespace Components.GameEntity.Movement
{
    public struct MoveSpeedScale : IComponentData
    {
        public float Value;
        public static readonly MoveSpeedScale DefaultValue = new MoveSpeedScale
        {
            Value = 1f,
        };
    }
}

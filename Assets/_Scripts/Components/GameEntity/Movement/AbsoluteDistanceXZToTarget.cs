using Unity.Entities;

namespace Components.GameEntity.Movement
{
    public struct AbsoluteDistanceXZToTarget : IComponentData
    {
        public float X;
        public float Z;
        public static AbsoluteDistanceXZToTarget MaxDistance = new()
        {
            X = float.MaxValue,
            Z = float.MaxValue,
        };
    }

}

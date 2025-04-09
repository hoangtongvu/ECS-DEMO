using Unity.Entities;

namespace Components
{
    public struct TargetPosChangedTag : IComponentData, IEnableableComponent
    {
    }

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

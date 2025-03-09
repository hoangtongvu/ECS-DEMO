using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.Tween
{
    public struct QuaternionTweenData : IComponentData
    {
        public float LifeTimeCounterSecond;
        public float BaseSpeed;
        public float4 Target;
    }

}

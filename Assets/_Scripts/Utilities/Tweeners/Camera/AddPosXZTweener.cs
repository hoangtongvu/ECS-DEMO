using Components.MyCamera;
using TweenLib;
using TweenLib.Utilities;
using TweenLib.Utilities.Helpers;
using Unity.Burst;
using Unity.Mathematics;

namespace Utilities.Tweeners.Camera
{
    [BurstCompile]
    public partial struct AddPosXZTweener : ITweener<AddPos, float2>
    {
        [BurstCompile]
        public void GetDefaultStartValue(in AddPos componentData, out float2 defaultStartValue)
            => defaultStartValue = componentData.Value.xz;

        [BurstCompile]
        public void GetSum(in float2 a, in float2 b, out float2 result)
            => result = a + b;

        [BurstCompile]
        public void GetDifference(in float2 a, in float2 b, out float2 result)
            => result = a - b;

        public void Tween(ref AddPos componentData, in float normalizedTime, EasingType easingType, in float2 startValue, in float2 target)
        {
            TweenHelper.Float2Tween(in normalizedTime, easingType, in startValue, in target, out var newValueXZ);
            componentData.Value.xz = newValueXZ;
        }

    }

}
using Components.MyCamera;
using TweenLib;
using TweenLib.Utilities;
using TweenLib.Utilities.Helpers;
using Unity.Burst;

namespace Utilities.Tweeners.MyCamera
{
    [BurstCompile]
    public partial struct AddPosYTweener : ITweener<AddPos, float>
    {
        [BurstCompile]
        public void GetDefaultStartValue(in AddPos componentData, out float defaultStartValue)
            => defaultStartValue = componentData.Value.y;

        [BurstCompile]
        public void GetSum(in float a, in float b, out float result)
            => result = a + b;

        [BurstCompile]
        public void GetDifference(in float a, in float b, out float result)
            => result = a - b;

        [BurstCompile]
        public void Tween(ref AddPos componentData, in float normalizedTime, EasingType easingType, in float startValue, in float target)
        {
            componentData.Value.y =
                TweenHelper.FloatTween(in normalizedTime, easingType, in startValue, in target);
        }

    }

}
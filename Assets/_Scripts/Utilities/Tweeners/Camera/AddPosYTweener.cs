using Components.Camera;
using TweenLib;
using Unity.Burst;
using Unity.Mathematics;

namespace Utilities.Tweeners.Camera
{
    [BurstCompile]
    public partial struct AddPosYTweener : ITweener<AddPos, float>
    {
        [BurstCompile]
        public bool CanStop(in AddPos componentData, in float lifeTimeSecond, in float baseSpeed, in float target)
        {
            return math.abs(target - componentData.Value.y) < Configs.Epsilon;
        }

        [BurstCompile]
        public void Tween(ref AddPos componentData, in float baseSpeed, in float target)
        {
            componentData.Value.y =
                math.lerp(componentData.Value.y, target, baseSpeed * this.DeltaTime);
        }

    }

}
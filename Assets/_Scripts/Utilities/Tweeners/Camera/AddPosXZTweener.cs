using Components.Camera;
using TweenLib;
using Unity.Burst;
using Unity.Mathematics;

namespace Utilities.Tweeners.Camera
{
    [BurstCompile]
    public partial struct AddPosXZTweener : ITweener<AddPos, float2>
    {
        [BurstCompile]
        public bool CanStop(in AddPos componentData, in float lifeTimeSecond, in float baseSpeed, in float2 target)
        {
            return math.all(math.abs(target - componentData.Value.xz) < new float2(Configs.Epsilon));
        }

        [BurstCompile]
        public void Tween(ref AddPos componentData, in float baseSpeed, in float2 target)
        {
            componentData.Value.xz =
                math.lerp(componentData.Value.xz, target, baseSpeed * this.DeltaTime); ;
        }

    }

}
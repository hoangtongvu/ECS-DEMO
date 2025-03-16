using Components.Camera;
using TweenLib;
using Unity.Burst;
using Unity.Mathematics;

namespace Utilities.Tweeners.Camera
{
    [BurstCompile]
    public partial struct AddPosTweener : ITweener<AddPos, float3>
    {
        [BurstCompile]
        public bool CanStop(in AddPos componentData, in float lifeTimeSecond, in float baseSpeed, in float3 target)
        {
            return math.all(math.abs(target - componentData.Value) < new float3(Configs.Epsilon));
        }

        [BurstCompile]
        public void Tween(ref AddPos componentData, in float baseSpeed, in float3 target)
        {
            componentData.Value =
                math.lerp(componentData.Value, target, baseSpeed * this.DeltaTime);
        }

    }

}
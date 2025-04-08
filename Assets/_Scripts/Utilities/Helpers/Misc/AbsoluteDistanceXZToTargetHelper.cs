using Components;
using Unity.Mathematics;
using Unity.Burst;

namespace Utilities.Helpers.Misc
{
    [BurstCompile]
    public static class AbsoluteDistanceXZToTargetHelper
    {
        [BurstCompile]
        public static void SetDistance(
            ref AbsoluteDistanceXZToTarget absoluteDistanceXZToTarget
            , in float3 currentPos
            , in float3 targetPos)
        {
            var tempDelta = targetPos - currentPos;
            absoluteDistanceXZToTarget.X = math.abs(tempDelta.x);
            absoluteDistanceXZToTarget.Z = math.abs(tempDelta.z);
        }

    }

}
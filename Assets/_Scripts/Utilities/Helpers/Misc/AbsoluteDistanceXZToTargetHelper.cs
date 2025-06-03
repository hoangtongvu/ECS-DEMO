using Unity.Mathematics;
using Unity.Burst;
using Components.GameEntity.Movement;

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
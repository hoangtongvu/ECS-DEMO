using Components.Unit;
using Unity.Burst;
using Unity.Entities;

namespace Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit
{
    [BurstCompile]
    public static partial class UpgradeAndRevertJoblessUnitHelper
    {
        [BurstCompile]
        public static void RemoveJoblessUnitTag(in EntityCommandBuffer ecb, in Entity unitEntity)
        {
            ecb.RemoveComponent<JoblessUnitTag>(unitEntity);
        }

        [BurstCompile]
        public static void AddJoblessUnitTag(in EntityCommandBuffer ecb, in Entity entity)
        {
            ecb.AddComponent<JoblessUnitTag>(entity);
        }

    }

}
using Components.Unit;
using Core.Unit;
using Unity.Burst;
using Unity.Entities;

namespace Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit
{
    public static partial class UpgradeAndRevertJoblessUnitHelper
    {
        [BurstCompile]
        public static void InitOnPick_Sword(
            in EntityCommandBuffer ecb
            , ref UnitProfileIdHolder unitProfileIdHolder
            , in Entity unitEntity)
        {
            unitProfileIdHolder.Value.UnitType = UnitType.Knight;
        }

        [BurstCompile]
        public static void RevertOnPick_Sword(
            in EntityCommandBuffer ecb
            , ref UnitProfileIdHolder unitProfileIdHolder
            , in Entity unitEntity)
        {
            unitProfileIdHolder.Value.UnitType = UnitType.Villager;
        }

    }

}
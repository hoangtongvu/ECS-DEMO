using Components.Unit;
using Core.Unit;
using Unity.Burst;
using Unity.Entities;

namespace Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit
{
    public static partial class UpgradeAndRevertJoblessUnitHelper
    {
        [BurstCompile]
        public static void InitOnPick_Bow(
            in EntityCommandBuffer ecb
            , ref UnitProfileIdHolder unitProfileIdHolder
            , in Entity unitEntity)
        {
            unitProfileIdHolder.Value.UnitType = UnitType.Archer;
        }

        [BurstCompile]
        public static void RevertOnPick_Bow(
            in EntityCommandBuffer ecb
            , ref UnitProfileIdHolder unitProfileIdHolder
            , in Entity unitEntity)
        {
            unitProfileIdHolder.Value.UnitType = UnitType.Villager;
        }

    }

}
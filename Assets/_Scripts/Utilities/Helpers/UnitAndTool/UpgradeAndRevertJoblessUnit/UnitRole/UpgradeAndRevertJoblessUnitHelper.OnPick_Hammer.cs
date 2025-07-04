using Components.Unit;
using Components.Unit.Misc;
using Core.Unit;
using Unity.Burst;
using Unity.Entities;

namespace Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit
{
    public static partial class UpgradeAndRevertJoblessUnitHelper
    {
        [BurstCompile]
        public static void InitOnPick_Hammer(
            in EntityCommandBuffer ecb
            , ref UnitProfileIdHolder unitProfileIdHolder
            , in Entity unitEntity)
        {
            unitProfileIdHolder.Value.UnitType = UnitType.Builder;

            ecb.AddComponent<IsBuilderUnitTag>(unitEntity);
        }

        [BurstCompile]
        public static void RevertOnPick_Hammer(
            in EntityCommandBuffer ecb
            , ref UnitProfileIdHolder unitProfileIdHolder
            , in Entity unitEntity)
        {
            unitProfileIdHolder.Value.UnitType = UnitType.Villager;

            ecb.RemoveComponent<IsBuilderUnitTag>(unitEntity);
        }

    }

}
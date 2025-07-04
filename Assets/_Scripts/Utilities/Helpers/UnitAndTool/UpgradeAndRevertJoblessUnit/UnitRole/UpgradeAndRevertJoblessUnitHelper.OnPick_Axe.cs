using Components.Harvest;
using Components.Unit;
using Core.Unit;
using Unity.Burst;
using Unity.Entities;

namespace Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit
{
    public static partial class UpgradeAndRevertJoblessUnitHelper
    {
        [BurstCompile]
        public static void InitOnPick_Axe(
            in EntityCommandBuffer ecb
            , ref UnitProfileIdHolder unitProfileIdHolder
            , in Entity unitEntity)
        {
            unitProfileIdHolder.Value.UnitType = UnitType.Harvester;

            ecb.AddComponent<HarvesterICD>(unitEntity);
            ecb.AddComponent<HarvesteeTypeHolder>(unitEntity);
        }

        [BurstCompile]
        public static void RevertOnPick_Axe(
            in EntityCommandBuffer ecb
            , ref UnitProfileIdHolder unitProfileIdHolder
            , in Entity unitEntity)
        {
            unitProfileIdHolder.Value.UnitType = UnitType.Villager;

            ecb.RemoveComponent<HarvesterICD>(unitEntity);
            ecb.RemoveComponent<HarvesteeTypeHolder>(unitEntity);
        }

    }

}
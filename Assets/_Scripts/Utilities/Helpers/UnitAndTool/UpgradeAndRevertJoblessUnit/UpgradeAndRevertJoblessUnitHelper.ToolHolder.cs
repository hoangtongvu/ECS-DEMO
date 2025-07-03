using Components.Tool.Misc;
using Components.Unit;
using Unity.Burst;

namespace Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit
{
    public static partial class UpgradeAndRevertJoblessUnitHelper
    {
        [BurstCompile]
        public static void SetToolHolder(
            ref UnitToolHolder unitToolHolder
            , in UnitToolHolder newUnitToolHolder
            , ref ToolProfileIdHolder toolProfileIdHolder
            , in ToolProfileIdHolder newtoolProfileIdHolder)
        {
            unitToolHolder = newUnitToolHolder;
            toolProfileIdHolder = newtoolProfileIdHolder;
        }

        [BurstCompile]
        public static void ResetToolHolder(
            ref UnitToolHolder unitToolHolder
            , ref ToolProfileIdHolder toolProfileIdHolder)
        {
            unitToolHolder = default;
            toolProfileIdHolder = default;
        }

    }

}
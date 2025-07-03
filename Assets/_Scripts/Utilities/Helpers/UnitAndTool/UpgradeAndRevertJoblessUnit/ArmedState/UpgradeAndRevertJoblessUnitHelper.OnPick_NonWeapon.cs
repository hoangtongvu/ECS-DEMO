using Components.GameEntity.Misc;
using Core.GameEntity.Misc;
using Unity.Burst;
using Unity.Entities;

namespace Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit
{
    public static partial class UpgradeAndRevertJoblessUnitHelper
    {
        [BurstCompile]
        public static void InitOnPick_NonWeapon(in EntityCommandBuffer ecb, in Entity unitEntity)
        {
            ecb.SetComponent(unitEntity, new ArmedStateHolder
            {
                Value = ArmedState.False,
            });
        }

        [BurstCompile]
        public static void RevertOnPick_NonWeapon(in EntityCommandBuffer ecb, in Entity unitEntity)
        {
        }

    }

}
using Components.GameEntity.Misc;
using Core.GameEntity.Misc;
using Unity.Burst;
using Unity.Entities;

namespace Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit
{
    public static partial class UpgradeAndRevertJoblessUnitHelper
    {
        [BurstCompile]
        public static void InitOnPick_Weapon(in EntityCommandBuffer ecb, in Entity unitEntity)
        {
            ecb.SetComponent(unitEntity, new ArmedStateHolder
            {
                Value = ArmedState.True,
            });

            ecb.RemoveComponent<IsUnarmedEntityTag>(unitEntity);
            ecb.AddComponent<IsArmedEntityTag>(unitEntity);
        }

        [BurstCompile]
        public static void RevertOnPick_Weapon(in EntityCommandBuffer ecb, in Entity unitEntity)
        {
            ecb.SetComponent(unitEntity, new ArmedStateHolder
            {
                Value = ArmedState.False,
            });

            ecb.AddComponent<IsUnarmedEntityTag>(unitEntity);
            ecb.RemoveComponent<IsArmedEntityTag>(unitEntity);
        }

    }

}
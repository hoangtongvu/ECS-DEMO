using Components.GameEntity.Misc;
using Core.GameEntity.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities.Extensions;

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
        public static void RevertOnPick_Weapon(in EntityManager em, in NativeArray<Entity> unitEntities)
        {
            em.SetComponentData(unitEntities, new ArmedStateHolder
            {
                Value = ArmedState.False,
            });

            em.AddComponent<IsUnarmedEntityTag>(unitEntities);
            em.RemoveComponent<IsArmedEntityTag>(unitEntities);
        }

    }

}
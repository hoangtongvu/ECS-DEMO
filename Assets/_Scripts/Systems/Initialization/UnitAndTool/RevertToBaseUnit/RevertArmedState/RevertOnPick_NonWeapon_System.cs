using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.RevertToBaseUnit;
using Unity.Burst;
using Unity.Entities;
using static Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit.UpgradeAndRevertJoblessUnitHelper;

namespace Systems.Initialization.UnitAndTool.RevertToBaseUnit.RevertArmedState
{
    [UpdateInGroup(typeof(RevertArmedStateSystemGroup))]
    [BurstCompile]
    public partial struct RevertOnPick_NonWeapon_System : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitToolHolder
                    , NeedRevertToBaseUnitTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (unitToolHolderRef, entity) in SystemAPI
                .Query<
                    RefRO<UnitToolHolder>>()
                .WithAll<
                    NeedRevertToBaseUnitTag>()
                .WithEntityAccess())
            {
                bool toolIsWeapon = SystemAPI.HasComponent<IsWeaponTag>(unitToolHolderRef.ValueRO.Value);
                if (toolIsWeapon) continue;

                RevertOnPick_NonWeapon(in ecb, in entity);

            }

        }

    }

}
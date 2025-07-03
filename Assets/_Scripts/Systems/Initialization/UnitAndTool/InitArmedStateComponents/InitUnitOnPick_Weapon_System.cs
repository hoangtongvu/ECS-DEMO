using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.Misc;
using Unity.Burst;
using Unity.Entities;
using static Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit.UpgradeAndRevertJoblessUnitHelper;

namespace Systems.Initialization.UnitAndTool.InitArmedStateComponents
{
    [UpdateInGroup(typeof(InitArmedStateComponentsSystemGroup))]
    [BurstCompile]
    public partial struct InitUnitOnPick_Weapon_System : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitToolHolder
                    , NeedInitArmedStateComponentsTag>()
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
                    NeedInitArmedStateComponentsTag>()
                .WithEntityAccess())
            {
                bool toolIsWeapon = SystemAPI.HasComponent<IsWeaponTag>(unitToolHolderRef.ValueRO.Value);
                if (!toolIsWeapon) continue;

                InitOnPick_Weapon(in ecb, in entity);

            }

        }

    }

}
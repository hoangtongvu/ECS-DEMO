using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.Misc;
using Core.Tool;
using Unity.Burst;
using Unity.Entities;
using static Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit.UpgradeAndRevertJoblessUnitHelper;

namespace Systems.Initialization.UnitAndTool.RoleUpdated.InitRoleComponents
{
    [UpdateInGroup(typeof(InitRoleComponentsSystemGroup))]
    [BurstCompile]
    public partial struct InitUnitOnPick_Basket_System : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ToolProfileIdHolder
                    , UnitProfileIdHolder
                    , NeedRoleUpdatedTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (toolProfileIdHolderRef, unitProfileIdHolderRef, entity) in SystemAPI
                .Query<
                    RefRO<ToolProfileIdHolder>
                    , RefRW<UnitProfileIdHolder>>()
                .WithAll<
                    NeedRoleUpdatedTag>()
                .WithEntityAccess())
            {
                if (toolProfileIdHolderRef.ValueRO.Value.ToolType != ToolType.Basket) continue;
                InitOnPick_Basket(in ecb, ref unitProfileIdHolderRef.ValueRW, in entity);
            }

        }

    }

}
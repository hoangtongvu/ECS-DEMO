using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.RevertToBaseUnit;
using Core.Tool;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using static Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit.UpgradeAndRevertJoblessUnitHelper;

namespace Systems.Initialization.UnitAndTool.RevertToBaseUnit.RevertRole
{
    [UpdateInGroup(typeof(RevertRoleSystemGroup))]
    [BurstCompile]
    public partial struct RevertUnitOnPick_Pickaxe_System : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ToolProfileIdHolder
                    , UnitProfileIdHolder
                    , NeedRevertToBaseUnitTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (toolProfileIdHolderRef, unitProfileIdHolderRef, entity) in SystemAPI
                .Query<
                    RefRO<ToolProfileIdHolder>
                    , RefRW<UnitProfileIdHolder>>()
                .WithAll<
                    NeedRevertToBaseUnitTag>()
                .WithEntityAccess())
            {
                if (toolProfileIdHolderRef.ValueRO.Value.ToolType != ToolType.Pickaxe) continue;
                RevertOnPick_Pickaxe(in ecb, ref unitProfileIdHolderRef.ValueRW, in entity);
            }

            ecb.Playback(state.EntityManager);

        }

    }

}
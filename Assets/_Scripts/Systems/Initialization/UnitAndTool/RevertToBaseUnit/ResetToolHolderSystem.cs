using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.RevertToBaseUnit;
using Unity.Burst;
using Unity.Entities;
using static Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit.UpgradeAndRevertJoblessUnitHelper;

namespace Systems.Initialization.UnitAndTool.RevertToBaseUnit
{
    [UpdateInGroup(typeof(RevertToBaseUnitSystemGroup))]
    [BurstCompile]
    public partial struct ResetToolHolderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitToolHolder
                    , ToolProfileIdHolder
                    , NeedRevertToBaseUnitTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (unitToolHolderRef, toolProfileIdHolderRef) in SystemAPI
                .Query<
                    RefRW<UnitToolHolder>
                    , RefRW<ToolProfileIdHolder>>()
                .WithAll<
                    NeedRevertToBaseUnitTag>())
            {
                ResetToolHolder(ref unitToolHolderRef.ValueRW, ref toolProfileIdHolderRef.ValueRW);
            }

        }

    }

}
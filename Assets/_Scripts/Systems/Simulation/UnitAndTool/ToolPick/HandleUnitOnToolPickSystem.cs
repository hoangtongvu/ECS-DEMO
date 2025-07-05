using Components.Misc;
using Components.Tool;
using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using static Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit.UpgradeAndRevertJoblessUnitHelper;

namespace Systems.Simulation.UnitAndTool.ToolPick
{
    [UpdateInGroup(typeof(ToolPickHandleSystemGroup))]
    [BurstCompile]
    public partial struct HandleUnitOnToolPickSystem : ISystem
    {
        private EntityQuery toolQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.toolQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    CanBePickedTag
                    , DerelictToolTag>()
                .WithAll<
                    ToolPickerEntity
                    , ToolProfileIdHolder>()
                .Build();

            var unitQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitToolHolder
                    , ToolProfileIdHolder
                    , BaseDmg
                    , BaseWorkSpeed>()
                .Build();

            state.RequireForUpdate(this.toolQuery);
            state.RequireForUpdate(unitQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var toolStatsMap = SystemAPI.GetSingleton<ToolStatsMap>();
            var em = state.EntityManager;

            var toolEntities = this.toolQuery.ToEntityArray(Allocator.Temp);
            var toolPickerEntities = this.toolQuery.ToComponentDataArray<ToolPickerEntity>(Allocator.Temp);
            var toolProfileIdHolders = this.toolQuery.ToComponentDataArray<ToolProfileIdHolder>(Allocator.Temp);
            int count = toolEntities.Length;

            for (int i = 0; i < count; i++)
            {
                var toolEntity = toolEntities[i];
                var unitEntity = toolPickerEntities[i].Value;
                var toolProfileIdHolder = toolProfileIdHolders[i];

                this.HandleUnit(
                    ref state
                    , in toolStatsMap
                    , in em
                    , in unitEntity
                    , in toolEntity
                    , in toolProfileIdHolder);

            }

        }

        [BurstCompile]
        private void HandleUnit(
            ref SystemState state
            , in ToolStatsMap toolStatsMap
            , in EntityManager em
            , in Entity unitEntity
            , in Entity toolEntity
            , in ToolProfileIdHolder toolProfileIdHolder)
        {
            var unitToolHolderRef = SystemAPI.GetComponentRW<UnitToolHolder>(unitEntity);
            var toolProfileIdHolderRef = SystemAPI.GetComponentRW<ToolProfileIdHolder>(unitEntity);

            SetToolHolder(
                ref unitToolHolderRef.ValueRW
                , new UnitToolHolder { Value = toolEntity }
                , ref toolProfileIdHolderRef.ValueRW
                , in toolProfileIdHolder);

            var toolStats = toolStatsMap.Value[toolProfileIdHolder.Value];

            SystemAPI.GetComponentRW<BaseDmg>(unitEntity).ValueRW.Value = toolStats.BaseDmg;
            SystemAPI.GetComponentRW<BaseWorkSpeed>(unitEntity).ValueRW.Value = toolStats.BaseWorkSpeed;

            RemoveJoblessUnitTag(in em, in unitEntity);

            em.AddComponent<NeedRoleUpdatedTag>(unitEntity);
            em.AddComponent<NeedInitArmedStateComponentsTag>(unitEntity);

        }

    }

}
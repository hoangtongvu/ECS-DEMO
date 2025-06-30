using Components.Harvest;
using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.Misc;
using Core.Tool;
using Core.Unit;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.UnitAndTool.RoleUpdated.InitRoleComponents
{
    [UpdateInGroup(typeof(InitRoleComponentsSystemGroup))]
    [BurstCompile]
    public partial struct InitUnitOnPick_Axe_System : ISystem
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

            new InitRoleComponentsJob
            {
                ECB = ecb.AsParallelWriter(),
            }.ScheduleParallel();

        }

        [WithAll(typeof(NeedRoleUpdatedTag))]
        [BurstCompile]
        private partial struct InitRoleComponentsJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ECB;

            [BurstCompile]
            void Execute(
                in ToolProfileIdHolder toolProfileIdHolder
                , ref UnitProfileIdHolder unitProfileIdHolder
                , Entity unitEntity
                , [EntityIndexInQuery] int entityIndexInQuery)
            {
                if (toolProfileIdHolder.Value.ToolType != ToolType.Axe) return;

                unitProfileIdHolder.Value.UnitType = UnitType.Harvester;

                this.ECB.AddComponent<HarvesterICD>(entityIndexInQuery, unitEntity);
                this.ECB.AddComponent<HarvesteeTypeHolder>(entityIndexInQuery, unitEntity);

            }

        }

    }

}
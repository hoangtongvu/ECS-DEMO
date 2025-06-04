using Components.Harvest;
using Components.Tool;
using Components.Unit;
using Components.Unit.Misc;
using Core.Tool;
using Core.Unit;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.Tool.InitRoleComponents
{
    [UpdateInGroup(typeof(InitRoleComponentsSystemGroup))]
    [BurstCompile]
    public partial struct InitUnitOnPick_Pickaxe_System : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ToolTypeICD
                    , UnitProfileIdHolder
                    , NeedInitRoleComponentsTag>()
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

        [WithAll(typeof(NeedInitRoleComponentsTag))]
        [BurstCompile]
        private partial struct InitRoleComponentsJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ECB;

            [BurstCompile]
            void Execute(
                in ToolTypeICD toolTypeHolder
                , ref UnitProfileIdHolder unitProfileIdHolder
                , Entity unitEntity
                , [EntityIndexInQuery] int entityIndexInQuery)
            {
                if (toolTypeHolder.Value != ToolType.Pickaxe) return;

                unitProfileIdHolder.Value.UnitType = UnitType.Harvester;

                this.ECB.AddComponent<HarvesterICD>(entityIndexInQuery, unitEntity);
                this.ECB.AddComponent<HarvesteeTypeHolder>(entityIndexInQuery, unitEntity);

                this.ECB.RemoveComponent<NeedInitRoleComponentsTag>(entityIndexInQuery, unitEntity);

            }

        }

    }

}
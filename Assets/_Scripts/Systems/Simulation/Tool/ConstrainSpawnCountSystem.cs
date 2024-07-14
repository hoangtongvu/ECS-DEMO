using Unity.Entities;
using Unity.Burst;
using Components.MyEntity.EntitySpawning;
using Systems.Simulation.MyEntity.EntitySpawning;
using Components.Tool;

namespace Systems.Simulation.Tool
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(IncSpawnCountSystem))]
    [BurstCompile]
    public partial struct ConstrainSpawnCountSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    EntitySpawningProfileElement
                    , ToolHoldLimit
                    , ToolHoldCount>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new ConstrainJob()
                .ScheduleParallel();
        }

        [BurstCompile]
        private partial struct ConstrainJob : IJobEntity
        {

            [BurstCompile]
            void Execute(
                DynamicBuffer<EntitySpawningProfileElement> spawningProfiles
                , in ToolHoldCount toolHoldCount
                , in ToolHoldLimit toolHoldLimit)
            {
                int profileLength = spawningProfiles.Length;
                int usedSlotCount = toolHoldCount.Value;
                int limit = toolHoldLimit.Value;

                for (int i = 0; i < profileLength; i++)
                {
                    ref var profile = ref spawningProfiles.ElementAt(i);

                    int total = usedSlotCount + profile.SpawnCount;

                    profile.CanIncSpawnCount = total < limit;
                }
            }

        }


    }
}
using Unity.Entities;
using Unity.Burst;
using Components.MyEntity.EntitySpawning;
using Components.Tool;
using Systems.Simulation.MyEntity.EntitySpawning;

namespace Systems.Simulation.Tool
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SpawnPrefabSystem))]
    [BurstCompile]
    public partial struct ToolHolderAddSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    SpawnerEntityRef
                    , NewlySpawnedTag>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var spawnerEntityRef in
                SystemAPI.Query<
                    RefRO<SpawnerEntityRef>>()
                    .WithAll<NewlySpawnedTag>())
            {
                Entity spawner = spawnerEntityRef.ValueRO.Value;

                var toolHoldCountRef = SystemAPI.GetComponentRW<ToolHoldCount>(spawner);
                var toolHoldLimitRef = SystemAPI.GetComponentRO<ToolHoldLimit>(spawner);

                if (toolHoldCountRef.ValueRO.Value >= toolHoldLimitRef.ValueRO.Value) continue;

                toolHoldCountRef.ValueRW.Value++;

            }

        }


    }
}
using Unity.Entities;
using Unity.Burst;
using Components.MyEntity.EntitySpawning;
using Components.Tool;
using Components;
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
                    SpawnerEntity
                    , SelfEntityRef
                    , NewlySpawnedTag>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var (spawnerEntityRef, selfEntityRef) in
                SystemAPI.Query<
                    RefRO<SpawnerEntity>
                    , RefRO<SelfEntityRef>>()
                    .WithAll<NewlySpawnedTag>())
            {
                Entity spawner = spawnerEntityRef.ValueRO.Value;

                var toolsHolder = SystemAPI.GetBuffer<ToolHolderElement>(spawner);
                var toolHoldLimitRef = SystemAPI.GetComponentRO<ToolHoldLimit>(spawner);

                if (toolsHolder.Length > toolHoldLimitRef.ValueRO.Value) continue;

                toolsHolder.Add(new ToolHolderElement
                {
                    Value = selfEntityRef.ValueRO.Value,
                });

            }

        }


    }
}
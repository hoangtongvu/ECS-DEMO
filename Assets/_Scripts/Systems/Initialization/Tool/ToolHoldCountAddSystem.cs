using Unity.Entities;
using Unity.Burst;
using Components.Tool;
using Components.GameEntity.EntitySpawning;
using Systems.Initialization.GameEntity.EntitySpawning;

namespace Systems.Initialization.Tool
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SpawnPrefabSystem))]
    [BurstCompile]
    public partial struct ToolHoldCountAddSystem : ISystem
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
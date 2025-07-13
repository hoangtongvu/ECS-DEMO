using Components.GameEntity.EntitySpawning;
using Components.Tool;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities.Extensions.GameEntity.EntitySpawning;

namespace Systems.Simulation.UnitAndTool.ToolPick
{
    [UpdateInGroup(typeof(ToolPickHandleSystemGroup))]
    [BurstCompile]
    public partial struct HandleToolSpawnerOnToolPickSystem : ISystem
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
                    SpawnerEntityHolder>()
                .Build();

            var toolSpawnerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    SpawnedEntityCounter
                    , SpawnedEntityArray>()
                .Build();

            state.RequireForUpdate(this.toolQuery);
            state.RequireForUpdate(toolSpawnerQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var toolEntities = this.toolQuery.ToEntityArray(Allocator.Temp);
            var spawnerEntityHolders = this.toolQuery.ToComponentDataArray<SpawnerEntityHolder>(Allocator.Temp);
            int count = toolEntities.Length;

            for (int i = 0; i < count; i++)
            {
                var toolEntity = toolEntities[i];
                var spawnerEntity = spawnerEntityHolders[i].Value;

                this.HandleToolSpawner(
                    ref state
                    , in spawnerEntity
                    , in toolEntity);

            }

        }

        [BurstCompile]
        private void HandleToolSpawner(
            ref SystemState state
            , in Entity spawnerEntity
            , in Entity toolEntity)
        {
            if (SystemAPI.HasComponent<SpawnedEntityCounter>(spawnerEntity))
            {
                var spawnedEntityCounterRef = SystemAPI.GetComponentRW<SpawnedEntityCounter>(spawnerEntity);
                spawnedEntityCounterRef.ValueRW.Value--;
            }

            if (SystemAPI.HasComponent<SpawnedEntityArray>(spawnerEntity))
            {
                var spawnedEntities = SystemAPI.GetComponent<SpawnedEntityArray>(spawnerEntity);
                spawnedEntities.Remove(in toolEntity);
            }
            
        }

    }

}
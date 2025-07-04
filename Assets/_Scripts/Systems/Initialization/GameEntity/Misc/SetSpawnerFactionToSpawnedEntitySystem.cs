using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Misc;
using Systems.Initialization.Misc;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Misc
{
    [UpdateInGroup(typeof(NewlySpawnedTagProcessSystemGroup))]
    [BurstCompile]
    public partial struct SetSpawnerFactionToSpawnedEntitySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    FactionIndex
                    , SpawnerEntityHolder
                    , NewlySpawnedTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (spawnedEntityfactionIndexRef, spawnerEntityHolderRef) in SystemAPI
                .Query<
                    RefRW<FactionIndex>
                    , RefRO<SpawnerEntityHolder>>()
                .WithAll<NewlySpawnedTag>())
            {
                var spawnerEntity = spawnerEntityHolderRef.ValueRO.Value;
                if (!SystemAPI.HasComponent<FactionIndex>(spawnerEntity)) continue;

                byte spawnerFactionIndex = SystemAPI.GetComponent<FactionIndex>(spawnerEntity).Value;
                spawnedEntityfactionIndexRef.ValueRW.Value = spawnerFactionIndex;

            }

        }

    }

}
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Misc;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.Unit.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct SetSpawnedUnitFactionIndexSystem : ISystem
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
            foreach (var (factionIndexRef, spawnerEntityHolderRef, entity) in SystemAPI
                .Query<
                    RefRW<FactionIndex>
                    , RefRO<SpawnerEntityHolder>>()
                .WithAll<NewlySpawnedTag>()
                .WithEntityAccess())
            {
                var spawnerEntity = spawnerEntityHolderRef.ValueRO.Value;
                if (!SystemAPI.HasComponent<FactionIndex>(spawnerEntity)) continue;

                byte spawnerFactionIndex = SystemAPI.GetComponent<FactionIndex>(spawnerEntity).Value;
                factionIndexRef.ValueRW.Value = spawnerFactionIndex;

            }

        }

    }

}
using Components.GameEntity.EntitySpawning;
using Unity.Burst;
using Unity.Entities;
using Utilities.Extensions;

namespace Systems.Simulation.GameEntity.EntitySpawning
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct AutoIncSpawnCountSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var entityQuery0 = SystemAPI.QueryBuilder()
                .WithAll<
                    EntitySpawningProfileElement
                    , SpawnedEntityCounter
                    , SpawnedEntityCountLimit
                    , SpawnerAutoSpawnTag>()
                    .Build();
            
            state.RequireForUpdate(entityQuery0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (spawningProfiles, spawnedEntityCounterRef, spawnedEntityCountLimitRef) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , RefRW<SpawnedEntityCounter>
                    , RefRO<SpawnedEntityCountLimit>>()
                    .WithAll<SpawnerAutoSpawnTag>())
            {
                int spawnCountLeft = spawnedEntityCountLimitRef.ValueRO.Value - spawnedEntityCounterRef.ValueRO.Value;
                int length = spawningProfiles.Length;
                int tempCounter = 0;

                while (spawnCountLeft > 0)
                {
                    ref var profile = ref spawningProfiles.ElementAt(tempCounter);

                    profile.SpawnCount.ChangeValue(profile.SpawnCount.Value + 1);
                    spawnedEntityCounterRef.ValueRW.Value++;

                    spawnCountLeft--;
                    tempCounter++;

                    if (tempCounter == length) tempCounter = 0;

                }

            }

        }

    }

}

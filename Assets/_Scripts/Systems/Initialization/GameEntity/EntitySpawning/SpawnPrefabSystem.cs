using Unity.Entities;
using Unity.Burst;
using Utilities.Extensions;
using Components.GameEntity.EntitySpawning;
using Unity.Collections;
using Unity.Jobs;
using Components.GameEntity.Misc;

namespace Systems.Initialization.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct SpawnPrefabSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var spawnerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    EntitySpawningProfileElement>()
                .Build();

            state.RequireForUpdate(spawnerQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            const int initialCap = 30;

            var toSpawnPrefabs = new NativeList<Entity>(initialCap, Allocator.Temp);
            var spawnerEntities = new NativeList<Entity>(initialCap, Allocator.Temp);

            foreach (var (profiles, spawnerEntity) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>>()
                    .WithEntityAccess())
            {
                for (int i = 0; i < profiles.Length; i++)
                {
                    ref var profile = ref profiles.ElementAt(i);

                    if (!profile.CanSpawnState) continue;
                    profile.CanSpawnState = false;

                    profile.SpawnCount.ChangeValue(profile.SpawnCount.Value - 1);

                    toSpawnPrefabs.Add(profile.PrefabToSpawn);
                    spawnerEntities.Add(spawnerEntity);
                    
                }

            }

            var em = state.EntityManager;
            int count = toSpawnPrefabs.Length;

            var spawnedEntities = new NativeArray<Entity>(count, Allocator.TempJob);

            for (int i = 0; i < count; i++)
            {
                spawnedEntities[i] = em.Instantiate(toSpawnPrefabs[i]);
                em.AddComponent<NeedInitPosAroundSpawnerTag>(spawnedEntities[i]);
            }

            state.Dependency = new SetComponentsJob
            {
                Entities = spawnedEntities,
                SpawnerEntities = spawnerEntities.ToArray(Allocator.TempJob),
                SpawnerEntityHolderLookup = SystemAPI.GetComponentLookup<SpawnerEntityHolder>(),
            }.ScheduleParallel(count, initialCap / 2, state.Dependency);

        }

        [BurstCompile]
        private struct SetComponentsJob : IJobParallelForBatch
        {
            [ReadOnly] [DeallocateOnJobCompletion]
            public NativeArray<Entity> Entities;

            [ReadOnly] [DeallocateOnJobCompletion]
            public NativeArray<Entity> SpawnerEntities;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<SpawnerEntityHolder> SpawnerEntityHolderLookup;

            [BurstCompile]
            public void Execute(int startIndex, int count)
            {
                int upperBound = startIndex + count;

                for (int i = 0; i < upperBound; i++)
                {
                    var entity = this.Entities[i];

                    if (!this.SpawnerEntityHolderLookup.HasComponent(entity)) continue;

                    var spawnerEntityHolderRef = this.SpawnerEntityHolderLookup.GetRefRW(entity);
                    spawnerEntityHolderRef.ValueRW.Value = this.SpawnerEntities[i];

                }

            }

        }

    }

}
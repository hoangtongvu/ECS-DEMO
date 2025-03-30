using Components;
using Components.MyEntity.EntitySpawning;
using Components.MyEntity.EntitySpawning.GlobalCostMap;
using Core.GameResource;
using Systems.Initialization.Misc.WorldMap;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.MyEntity.EntitySpawning.GlobalCostMap
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(MapChangedSystemGroup))] // To perform structure change right after structure change in BuildCommandsExecutorSystem.
    [BurstCompile]
    public partial struct SpawningCostMapInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var su = SingletonUtilities.GetInstance(state.EntityManager);

            su.AddOrSetComponentData(new LatestCostMapIndex
            {
                Value = -1,
            });

            su.AddOrSetComponentData(new EntityToCostMapIndexMap
            {
                Value = new(20, Allocator.Persistent),
            });

            su.AddOrSetComponentData(new EntitySpawningCostsContainer
            {
                Value = new(60, Allocator.Persistent),
            });

            var query = SystemAPI.QueryBuilder()
                .WithAll<EntitySpawningProfileElement>()
                .WithAll<LocalCostMapElement>()
                .Build();

            state.RequireForUpdate(query);
            state.RequireForUpdate<EnumLength<ResourceType>>();

            state.Enabled = false;

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int resourceCount = SystemAPI.GetSingleton<EnumLength<ResourceType>>().Value;
            var latestCostMapIndexRef = SystemAPI.GetSingletonRW<LatestCostMapIndex>();
            var entityToCostMapIndexMap = SystemAPI.GetSingleton<EntityToCostMapIndexMap>();
            var entitySpawningCostsContainer = SystemAPI.GetSingleton<EntitySpawningCostsContainer>();

            var spawnerEntities = new NativeList<Entity>(5, Allocator.Temp);

            foreach (var (profiles, localCosts, spawnerEntity) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , DynamicBuffer<LocalCostMapElement>>()
                    .WithEntityAccess())
            {
                spawnerEntities.Add(spawnerEntity);

                int profileCount = profiles.Length;

                for (int i = 0; i < profileCount; i++)
                {
                    var profile = profiles[i];
                    int nextMapIndex = latestCostMapIndexRef.ValueRO.Value + 1;

                    if (!entityToCostMapIndexMap.Value.TryAdd(profile.PrefabToSpawn, nextMapIndex)) continue;

                    latestCostMapIndexRef.ValueRW.Value++;

                    for (int j = i * resourceCount; j < i * resourceCount + resourceCount; j++)
                    {
                        entitySpawningCostsContainer.Value.Add(localCosts[j].Cost);
                    }

                }

            }

            this.RemoveLocalCostBuffer(ref state, in spawnerEntities);
            spawnerEntities.Dispose();

        }

        [BurstCompile]
        private void RemoveLocalCostBuffer(
            ref SystemState state
            , in NativeList<Entity> entities)
        {
            state.EntityManager.RemoveComponent<LocalCostMapElement>(entities.AsArray());
        }

    }

}
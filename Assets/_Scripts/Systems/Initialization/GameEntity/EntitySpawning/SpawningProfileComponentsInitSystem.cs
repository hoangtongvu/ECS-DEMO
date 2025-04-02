using Components;
using Components.GameEntity;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Core.GameResource;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;
using Utilities;

namespace Systems.Initialization.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SceneSystemGroup))]
    [BurstCompile]
    public partial struct SpawningProfileComponentsInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var su = SingletonUtilities.GetInstance(state.EntityManager);

            su.AddOrSetComponentData(new LatestCostMapIndex
            {
                Value = -1,
            });

            su.AddOrSetComponentData(new EntityToContainerIndexMap
            {
                Value = new(20, Allocator.Persistent),
            });

            su.AddOrSetComponentData(new EntitySpawningCostsContainer
            {
                Value = new(60, Allocator.Persistent),
            });

            su.AddOrSetComponentData(new EntitySpawningDurationsContainer
            {
                Value = new(20, Allocator.Persistent),
            });

            su.AddOrSetComponentData(new EntitySpawningSpritesContainer
            {
                Value = new(20, Allocator.Persistent),
            });

            state.RequireForUpdate<EnumLength<ResourceType>>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var latestCostMapIndexRef = SystemAPI.GetSingletonRW<LatestCostMapIndex>();
            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var entitySpawningCostsContainer = SystemAPI.GetSingleton<EntitySpawningCostsContainer>();
            var durationsContainer = SystemAPI.GetSingleton<EntitySpawningDurationsContainer>();
            var spritesContainer = SystemAPI.GetSingleton<EntitySpawningSpritesContainer>();
            int resourceCount = SystemAPI.GetSingleton<EnumLength<ResourceType>>().Value;

            foreach (var (bakedProfiles, localProfilePictures, localSpawningDurations, localSpawningCosts) in
                SystemAPI.Query<
                    DynamicBuffer<BakedGameEntityProfileElement>
                    , DynamicBuffer<LocalSpawningProfilePictureElement>
                    , DynamicBuffer<LocalSpawningDurationSecondsElement>
                    , DynamicBuffer<LocalSpawningCostElement>>())
            {
                int profileCount = bakedProfiles.Length;

                for (int i = 0; i < profileCount; i++)
                {
                    int nextMapIndex = latestCostMapIndexRef.ValueRO.Value + 1;

                    var key = bakedProfiles[i].PrimaryEntity;
                    if (key == Entity.Null) continue;

                    var value = nextMapIndex;

                    if (!entityToContainerIndexMap.Value.TryAdd(key, value)) continue;

                    spritesContainer.Value.Add(localProfilePictures[i].Value);
                    durationsContainer.Value.Add(localSpawningDurations[i].Value);

                    for (int j = i * resourceCount; j < (i + 1) * resourceCount; j++)
                    {
                        entitySpawningCostsContainer.Value.Add(localSpawningCosts[j].Value);
                    }

                    latestCostMapIndexRef.ValueRW.Value++;

                }

            }

        }

    }

}
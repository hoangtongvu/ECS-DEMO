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
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    BakedGameEntityProfileElement
                    , LocalSpawningProfilePictureElement
                    , LocalSpawningDurationSecondsElement
                    , LocalSpawningCostElement>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            int latestMapIndex = -1;

            var entityToContainerIndexMap = new EntityToContainerIndexMap
            {
                Value = new(20, Allocator.Persistent),
            };
            var entitySpawningCostsContainer = new EntitySpawningCostsContainer
            {
                Value = new(60, Allocator.Persistent),
            };
            var durationsContainer = new EntitySpawningDurationsContainer
            {
                Value = new(20, Allocator.Persistent),
            };
            var spritesContainer = new EntitySpawningSpritesContainer
            {
                Value = new(20, Allocator.Persistent),
            };
            
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
                    int nextMapIndex = latestMapIndex + 1;

                    var key = bakedProfiles[i].PrimaryEntity;
                    if (key == Entity.Null) continue;

                    var value = nextMapIndex;

                    if (!entityToContainerIndexMap.Value.TryAdd(key, value)) continue;

                    spritesContainer.Value.Add(localProfilePictures[i].Value);
                    durationsContainer.Value.Add(localSpawningDurations[i].Value);

                    for (int j = i * ResourceType_Length.Value; j < (i + 1) * ResourceType_Length.Value; j++)
                    {
                        entitySpawningCostsContainer.Value.Add(localSpawningCosts[j].Value);
                    }

                    latestMapIndex++;

                }

            }

            var su = SingletonUtilities.GetInstance(state.EntityManager);
            su.AddOrSetComponentData(entityToContainerIndexMap);
            su.AddOrSetComponentData(entitySpawningCostsContainer);
            su.AddOrSetComponentData(durationsContainer);
            su.AddOrSetComponentData(spritesContainer);

        }

    }

}
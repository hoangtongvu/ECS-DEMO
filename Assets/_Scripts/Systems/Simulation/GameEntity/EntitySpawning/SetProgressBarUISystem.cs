using Unity.Entities;
using Components;
using Core.MyEvent.PubSub.Messengers;
using ZBase.Foundation.PubSub;
using Core.MyEvent.PubSub.Messages;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using System.Collections.Generic;
using Components.Misc;

namespace Systems.Simulation.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SetProgressBarUISystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    WithinPlayerAutoInteractRadiusTag
                    , EntitySpawningProfileElement
                    , UISpawned>()
                .Build();

            this.RequireForUpdate(query);
            this.RequireForUpdate<EntityToContainerIndexMap>();
            this.RequireForUpdate<EntitySpawningDurationsContainer>();

        }

        protected override void OnUpdate()
        {
            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var durationsContainer = SystemAPI.GetSingleton<EntitySpawningDurationsContainer>();

            foreach (var (spawningProfiles, uiSpawnedRef, spawnerEntity) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , RefRO<UISpawned>>()
                    .WithAll<WithinPlayerAutoInteractRadiusTag>()
                    .WithEntityAccess())
            {
                if (!uiSpawnedRef.ValueRO.IsSpawned) continue;

                int profileCount = spawningProfiles.Length;

                for (int i = 0; i < profileCount; i++)
                {
                    var profile = spawningProfiles[i];

                    if (profile.SpawnCount.Value <= 0) continue;

                    float spawnDurationSeconds = this.GetDurationSeconds(
                        in entityToContainerIndexMap
                        , in durationsContainer
                        , in profile.PrefabToSpawn);

                    // This means when nothing to spawned, UI won't update.
                    float progressValue =
                        profile.DurationCounterSeconds / spawnDurationSeconds;

                    GameplayMessenger.MessagePublisher.Publish(new SetProgressBarMessage
                    {
                        SpawnerEntity = spawnerEntity,
                        SpawningProfileElementIndex = i,
                        Value = progressValue,
                    });

                }

            }

        }

        private float GetDurationSeconds(
            in EntityToContainerIndexMap entityToContainerIndexMap
            , in EntitySpawningDurationsContainer durationsContainer
            , in Entity prefabToSpawnEntity)
        {
            if (!entityToContainerIndexMap.Value.TryGetValue(prefabToSpawnEntity, out int containerIndex))
                throw new KeyNotFoundException($"{nameof(EntityToContainerIndexMap)} does not contain key: {prefabToSpawnEntity}");

            return durationsContainer.Value[containerIndex];
        }

    }

}
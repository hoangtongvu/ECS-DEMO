using Components.GameEntity.EntitySpawning;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.GameEntity.InteractableActions;
using Components.GameEntity.Misc;
using Components.Player;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using System.Collections.Generic;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Simulation.GameEntity.EntitySpawning.InteractableActions
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SetProgressBarUISystem : SystemBase
    {
        private EntityQuery playerQuery;

        protected override void OnCreate()
        {
            this.playerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , FactionIndex>()
                .Build();

            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    EntitySpawningProfileElement
                    , ActionsContainerUIShownTag>()
                .Build();

            this.RequireForUpdate(query);
            this.RequireForUpdate<EntityToContainerIndexMap>();
            this.RequireForUpdate<EntitySpawningDurationsContainer>();

        }

        protected override void OnUpdate()
        {
            var playerFactionIndex = this.playerQuery.GetSingleton<FactionIndex>();
            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var durationsContainer = SystemAPI.GetSingleton<EntitySpawningDurationsContainer>();

            foreach (var (factionIndexRef, spawningProfiles, spawnerEntity) in SystemAPI
                .Query<
                    RefRO<FactionIndex>
                    , DynamicBuffer<EntitySpawningProfileElement>>()
                .WithAll<ActionsContainerUIShownTag>()
                .WithEntityAccess())
            {
                if (factionIndexRef.ValueRO.Value != playerFactionIndex.Value) continue;

                int profileCount = spawningProfiles.Length;

                for (int i = 0; i < profileCount; i++)
                {
                    var profile = spawningProfiles[i];

                    if (profile.SpawnCount.Value <= 0) continue;

                    float spawnDurationSeconds = this.GetDurationSeconds(
                        in entityToContainerIndexMap
                        , in durationsContainer
                        , in profile.PrefabToSpawn);

                    // NOTE: This means when nothing to spawned, UI won't update.
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
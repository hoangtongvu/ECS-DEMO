using Unity.Burst;
using Unity.Entities;
using Components.GameResource;
using Core.MyEvent.PubSub.Messages;
using Utilities.Extensions;
using Components.Player;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.Misc;
using Utilities.Helpers;

namespace Systems.Simulation.GameEntity.EntitySpawning
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct PlayerIncSpawnCountSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var entityQuery0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ResourceWalletElement
                    , WalletChangedTag>()
                    .Build();

            var entityQuery1 = SystemAPI.QueryBuilder()
                .WithAll<
                    MessageQueue<SpawnUnitMessage>>()
                    .Build();

            var entityQuery2 = SystemAPI.QueryBuilder()
                .WithAll<
                    WithinPlayerAutoInteractRadiusTag
                    , EntitySpawningProfileElement>()
                    .Build();
            
            var entityQuery3 = SystemAPI.QueryBuilder()
                .WithAll<
                    EntityToContainerIndexMap
                    , EntitySpawningCostsContainer>()
                    .Build();

            state.RequireForUpdate(entityQuery0);
            state.RequireForUpdate(entityQuery1);
            state.RequireForUpdate(entityQuery2);
            state.RequireForUpdate(entityQuery3);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var messageQueue = SystemAPI.GetSingleton<MessageQueue<SpawnUnitMessage>>();

            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var entitySpawningCostsContainer = SystemAPI.GetSingleton<EntitySpawningCostsContainer>();

            this.GetWallet(ref state, out var resourceWallet, out var walletChangedTag);

            while (messageQueue.Value.TryDequeue(out var message))
            {
                var profiles = SystemAPI.GetBuffer<EntitySpawningProfileElement>(message.SpawnerEntity);
                var spawnedEntityCounterRef = SystemAPI.GetComponentRW<SpawnedEntityCounter>(message.SpawnerEntity);
                int spawnedEntityCountLimit = SystemAPI.GetComponent<SpawnedEntityCountLimit>(message.SpawnerEntity).Value;

                bool canIncSpawnCount = spawnedEntityCounterRef.ValueRO.Value < spawnedEntityCountLimit;
                if (!canIncSpawnCount) continue;

                ref var profile = ref profiles.ElementAt(message.SpawningProfileElementIndex);

                bool canSpendResources = ResourceWalletHelper.TrySpendResources(
                    ref resourceWallet
                    , ref walletChangedTag
                    , in entityToContainerIndexMap
                    , in entitySpawningCostsContainer
                    , in profile.PrefabToSpawn);

                if (!canSpendResources) continue;

                profile.SpawnCount.ChangeValue(profile.SpawnCount.Value + 1);
                spawnedEntityCounterRef.ValueRW.Value++;

            }

        }

        [BurstCompile]
        private void GetWallet(
            ref SystemState state
            , out DynamicBuffer<ResourceWalletElement> resourceWallet
            , out EnabledRefRW<WalletChangedTag> walletChangedTag)
        {
            resourceWallet = default;
            walletChangedTag = default;

            foreach (var item in
                SystemAPI.Query<
                    DynamicBuffer<ResourceWalletElement>
                    , EnabledRefRW<WalletChangedTag>>()
                    .WithAll<PlayerTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                resourceWallet = item.Item1;
                walletChangedTag = item.Item2;
            }

        }

    }

}

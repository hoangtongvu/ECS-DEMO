using Components.MyEntity.EntitySpawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Components.GameResource;
using Core.GameResource;
using Components;
using Core.MyEvent.PubSub.Messages;
using Utilities.Extensions;
using Components.Unit.UnitSelection;
using Components.Player;
using System.Collections.Generic;
using Components.MyEntity.EntitySpawning.SpawningProfiles;
using Components.MyEntity.EntitySpawning.SpawningProfiles.Containers;

namespace Systems.Simulation.MyEntity.EntitySpawning
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct IncSpawnCountSystem : ISystem
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
                    MessageQueue<SpawnUnitMessage>
                    , EnumLength<ResourceType>>()
                    .Build();

            var entityQuery2 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitSelectedTag
                    , EntitySpawningProfileElement>()
                    .Build();
            
            var entityQuery3 = SystemAPI.QueryBuilder()
                .WithAll<
                    EntityToCostMapIndexMap
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
            var resourceCount = SystemAPI.GetSingleton<EnumLength<ResourceType>>();

            var entityToCostMapIndexMap = SystemAPI.GetSingleton<EntityToCostMapIndexMap>();
            var entitySpawningCostsContainer = SystemAPI.GetSingleton<EntitySpawningCostsContainer>();

            DynamicBuffer<ResourceWalletElement> resourceWallet = default;
            EnabledRefRW<WalletChangedTag> walletChangedTag = default;

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

            while (messageQueue.Value.TryDequeue(out var message))
            {
                var profiles = SystemAPI.GetBuffer<EntitySpawningProfileElement>(message.SpawnerEntity);

                ref var profile = ref profiles.ElementAt(message.SpawningProfileElementIndex);

                if (!this.HaveEnoughResources(
                    resourceWallet
                    , in entityToCostMapIndexMap
                    , in entitySpawningCostsContainer
                    , in profile.PrefabToSpawn
                    , resourceCount.Value
                    , out var walletArr)) continue;

                resourceWallet.CopyFrom(walletArr);
                walletChangedTag.ValueRW = true;

                walletArr.Dispose();

                profile.SpawnCount.ChangeValue(profile.SpawnCount.Value + 1);

            }

        }

        [BurstCompile]
        private bool HaveEnoughResources(
            DynamicBuffer<ResourceWalletElement> resourceWallet
            , in EntityToCostMapIndexMap entityToCostMapIndexMap
            , in EntitySpawningCostsContainer entitySpawningCostsContainer
            , in Entity prefabEntity
            , int resourceCount
            , out NativeArray<ResourceWalletElement> walletArr)
        {
            walletArr = resourceWallet.ToNativeArray(Allocator.Temp);

            int length = walletArr.Length;

            for (int i = 0; i < length; i++)
            {
                ResourceType resourceType = (ResourceType)i;

                uint cost = this.GetCost(
                    in entityToCostMapIndexMap
                    , in entitySpawningCostsContainer
                    , in prefabEntity
                    , resourceCount
                    , resourceType);

                long tempValue = (long) walletArr[i].Quantity - cost;
                // UnityEngine.Debug.Log($"{resourceType} {tempValue} = {walletArr[i].Quantity} - {cost}");

                if (tempValue < 0) return false;
                walletArr[i] = new ResourceWalletElement
                {
                    Type = resourceType,
                    Quantity = (uint)tempValue,
                };

            }

            return true;
        }

        [BurstCompile]
        private uint GetCost(
            in EntityToCostMapIndexMap entityToCostMapIndexMap
            , in EntitySpawningCostsContainer entitySpawningCostsContainer
            , in Entity prefabEntity
            , int resourceCount
            , ResourceType resourceType)
        {
            if (!entityToCostMapIndexMap.Value.TryGetValue(prefabEntity, out int costMapIndex))
                throw new KeyNotFoundException($"{nameof(entityToCostMapIndexMap)} does not contain key: {prefabEntity}");

            int costIndexInContainer = costMapIndex * resourceCount + (int)resourceType;
            return entitySpawningCostsContainer.Value[costIndexInContainer];

        }

    }

}

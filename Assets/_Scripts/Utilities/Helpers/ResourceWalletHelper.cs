using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameResource;
using Core.GameResource;
using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Utilities.Helpers
{
    [BurstCompile]
    public static class ResourceWalletHelper
    {
        [BurstCompile]
        public static bool TryAddResources(
            in ComponentLookup<WalletChangedTag> walletChangedTagLookup
            , in Entity walletOwnerEntity
            , in DynamicBuffer<ResourceWalletElement> resourceWallet
            , ResourceType addType
            , uint addQuantity)
        {
            int walletLength = resourceWallet.Length;

            for (int i = 0; i < walletLength; i++)
            {
                ref var walletElement = ref resourceWallet.ElementAt(i);

                bool matchType = walletElement.Type == addType;
                if (!matchType) continue;

                walletElement.Quantity += addQuantity;
                walletChangedTagLookup.SetComponentEnabled(walletOwnerEntity, true);

                return true;
            }

            return false;
        }

        public static DynamicBuffer<ResourceWalletElement> AddResourceWalletToEntity(IBaker baker, Entity entity)
        {
            int length = Enum.GetNames(typeof(ResourceType)).Length;
            var resourceWallet = baker.AddBuffer<ResourceWalletElement>(entity);

            for (int i = 0; i < length; i++)
            {
                var type = (ResourceType)i;

                resourceWallet.Add(new ResourceWalletElement
                {
                    Type = type,
                    Quantity = 0,
                });

            }

            return resourceWallet;
        }

        [BurstCompile]
        public static bool TrySpendResources(
            ref DynamicBuffer<ResourceWalletElement> resourceWallet
            , ref EnabledRefRW<WalletChangedTag> walletChangedTag
            , in EntityToContainerIndexMap entityToContainerIndexMap
            , in EntitySpawningCostsContainer entitySpawningCostsContainer
            , in Entity prefabEntity)
        {
            var walletArr = resourceWallet.ToNativeArray(Allocator.Temp);
            int length = walletArr.Length;

            for (int i = 0; i < length; i++)
            {
                ResourceType resourceType = (ResourceType)i;

                uint cost = GetCostFromResourceType(
                    in entityToContainerIndexMap
                    , in entitySpawningCostsContainer
                    , in prefabEntity
                    , resourceType);

                long tempQuantity = (long)walletArr[i].Quantity - cost;
                //UnityEngine.Debug.Log($"{resourceType} {tempQuantity} = {walletArr[i].Quantity} - {cost}");

                bool enoughResource = tempQuantity >= 0;
                if (!enoughResource)
                {
                    walletArr.Dispose();
                    return false;
                }

                walletArr[i] = new ResourceWalletElement
                {
                    Type = resourceType,
                    Quantity = (uint)tempQuantity,
                };

            }

            resourceWallet.CopyFrom(walletArr);
            walletChangedTag.ValueRW = true;
            walletArr.Dispose();

            return true;
        }

        [BurstCompile]
        private static uint GetCostFromResourceType(
            in EntityToContainerIndexMap entityToContainerIndexMap
            , in EntitySpawningCostsContainer entitySpawningCostsContainer
            , in Entity prefabEntity
            , ResourceType resourceType)
        {
            if (!entityToContainerIndexMap.Value.TryGetValue(prefabEntity, out int costMapIndex))
                throw new KeyNotFoundException($"{nameof(entityToContainerIndexMap)} does not contain key: {prefabEntity}");

            int costIndexInContainer = costMapIndex * ResourceType_Length.Value + (int)resourceType;
            return entitySpawningCostsContainer.Value[costIndexInContainer];

        }

    }

}
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameResource;
using Core.GameResource;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Utilities.Helpers
{
    [BurstCompile]
    public static class ResourceWalletHelper
    {
        [BurstCompile]
        public static bool TryAddResourceOfType(
            in DynamicBuffer<ResourceWalletElement> resourceWallet
            , EnabledRefRW<WalletChangedTag> walletChangedTag
            , ResourceType resourceType
            , uint addQuantity)
        {
            int walletLength = resourceWallet.Length;

            for (int i = 0; i < walletLength; i++)
            {
                ref var walletElement = ref resourceWallet.ElementAt(i);

                bool matchType = walletElement.Type == resourceType;
                if (!matchType) continue;

                walletElement.Quantity += addQuantity;
                walletChangedTag.ValueRW = true;

                return true;
            }

            return false;
        }

        public static DynamicBuffer<ResourceWalletElement> AddResourceWalletToEntity(
            IBaker baker
            , Entity entity
            , uint quantityPerElement = 0)
        {
            var resourceWallet = baker.AddBuffer<ResourceWalletElement>(entity);

            InitResourceWallet(ref resourceWallet, quantityPerElement);

            return resourceWallet;
        }

        public static DynamicBuffer<ResourceWalletElement> AddResourceWalletToEntity(
            in EntityManager em
            , in Entity entity
            , uint quantityPerElement = 0)
        {
            var resourceWallet = em.AddBuffer<ResourceWalletElement>(entity);

            InitResourceWallet(ref resourceWallet, quantityPerElement);

            return resourceWallet;
        }

        private static void InitResourceWallet(ref DynamicBuffer<ResourceWalletElement> resourceWallet, uint quantityPerElement)
        {
            for (int i = 0; i < ResourceType_Length.Value; i++)
            {
                var type = (ResourceType)i;

                resourceWallet.Add(new ResourceWalletElement
                {
                    Type = type,
                    Quantity = quantityPerElement,
                });
            }

        }

        [BurstCompile]
        public static bool TrySpendResourceOfType(
            ref DynamicBuffer<ResourceWalletElement> resourceWallet
            , ref EnabledRefRW<WalletChangedTag> walletChangedTag
            , ResourceType resourceType
            , in uint spendAmount)
        {
            ref var walletElement = ref resourceWallet.ElementAt((int)resourceType);
            uint currentAmount = walletElement.Quantity;

            if (spendAmount > currentAmount) return false;

            walletElement.Quantity = currentAmount - spendAmount;
            walletChangedTag.ValueRW = true;

            return true;
        }

        [BurstCompile]
        public static bool TrySpendResources(
            ref DynamicBuffer<ResourceWalletElement> resourceWallet
            , ref EnabledRefRW<WalletChangedTag> walletChangedTag
            , in EntityToContainerIndexMap entityToContainerIndexMap
            , in EntitySpawningCostsContainer entitySpawningCostsContainer
            , in Entity primaryEntity)
        {
            var walletArr = resourceWallet.ToNativeArray(Allocator.Temp);

            bool canSpendResources = TrySpendResources(
                ref walletArr
                , in entityToContainerIndexMap
                , in entitySpawningCostsContainer
                , in primaryEntity);

            if (!canSpendResources) return false;

            resourceWallet.CopyFrom(walletArr);
            walletChangedTag.ValueRW = true;

            return true;
        }

        /// <summary>
        /// This is not really a "Try" method, as it modifies the input <c>walletArr</c> regardless of the result.
        /// This method assumes that <c>walletArr</c> and <c>costs</c> having the same size and <c>ResourceType</c> order.
        /// </summary>
        [BurstCompile]
        public static bool TrySpendResources(
            ref NativeArray<ResourceWalletElement> walletArr
            , in EntityToContainerIndexMap entityToContainerIndexMap
            , in EntitySpawningCostsContainer entitySpawningCostsContainer
            , in Entity primaryEntity)
        {
            GetCostsSlice(
                in entityToContainerIndexMap
                , in entitySpawningCostsContainer
                , in primaryEntity
                , out var costs);

            return TrySpendResources(ref walletArr, in costs);
        }

        /// <summary>
        /// This is not really a "Try" method, as it modifies the input <c>walletArr</c> regardless of the result.
        /// This method assumes that <c>walletArr</c> and <c>costs</c> having the same size and <c>ResourceType</c> order.
        /// </summary>
        [BurstCompile]
        public static bool TrySpendResources(
            ref NativeArray<ResourceWalletElement> walletArr
            , in NativeSlice<uint> costs)
        {
            int length = walletArr.Length;

            for (int i = 0; i < length; i++)
            {
                ResourceType resourceType = (ResourceType)i;

                long tempQuantity = (long)walletArr[i].Quantity - costs[i];

                bool enoughResource = tempQuantity >= 0;
                if (!enoughResource) return false;

                walletArr[i] = new ResourceWalletElement
                {
                    Type = resourceType,
                    Quantity = (uint)tempQuantity,
                };
            }

            return true;
        }

        [BurstCompile]
        public static void GetCostsSlice(
            in EntityToContainerIndexMap entityToContainerIndexMap
            , in EntitySpawningCostsContainer entitySpawningCostsContainer
            , in Entity entity
            , out NativeSlice<uint> costs)
        {
            int costMapIndex = GetCostMapIndex(in entityToContainerIndexMap, entity);

            costs = new NativeSlice<uint>(
                entitySpawningCostsContainer.Value.AsArray()
                , costMapIndex * ResourceType_Length.Value
                , ResourceType_Length.Value);

        }

        [BurstCompile]
        private static int GetCostMapIndex(
            in EntityToContainerIndexMap entityToContainerIndexMap
            , in Entity entity)
        {
            return entityToContainerIndexMap.Value[entity];
        }

    }

}
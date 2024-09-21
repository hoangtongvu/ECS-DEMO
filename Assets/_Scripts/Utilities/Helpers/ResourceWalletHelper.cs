using Components.GameResource;
using Core.GameResource;
using System;
using Unity.Burst;
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
    }
}
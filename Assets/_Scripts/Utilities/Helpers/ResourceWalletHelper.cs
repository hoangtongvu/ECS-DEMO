using Components.GameResource;
using Core.GameResource;
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
    }
}
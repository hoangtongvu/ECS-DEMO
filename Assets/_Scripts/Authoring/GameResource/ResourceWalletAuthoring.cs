using Components.GameResource;
using Unity.Entities;
using UnityEngine;
using Utilities.Helpers;

namespace Authoring.GameResource
{

    public class ResourceWalletAuthoring : MonoBehaviour
    {

        private class Baker : Baker<ResourceWalletAuthoring>
        {
            public override void Bake(ResourceWalletAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                var resourceWallet = ResourceWalletHelper.AddResourceWalletToEntity(this, entity);
                int walletLength = resourceWallet.Length;

                for (int i = 0; i < walletLength; i++)
                {
                    resourceWallet.ElementAt(i).Quantity = 100;
                }

                AddComponent<WalletChangedTag>(entity);
                SetComponentEnabled<WalletChangedTag>(entity, false);

            }
        }
    }
}

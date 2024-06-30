using Components.GameResource;
using Core.GameResource;
using System;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameResource
{

    public class ResourceWalletAuthoring : MonoBehaviour
    {

        private class Baker : Baker<ResourceWalletAuthoring>
        {
            public override void Bake(ResourceWalletAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                int length = Enum.GetNames(typeof(ResourceType)).Length;

                var resourceWallet = AddBuffer<ResourceWalletElement>(entity);

                for (int i = 0; i < length; i++)
                {
                    var type = (ResourceType) i;

                    resourceWallet.Add(new ResourceWalletElement
                    {
                        Type = type,
                        Quantity = 10,
                    });

                }

                // AddBuffer<ResourceWalletChangedElement>(entity);
                AddComponent<WalletChanged>(entity);

            }
        }
    }
}

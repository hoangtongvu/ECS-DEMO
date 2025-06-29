using Core.GameResource;
using Unity.Entities;

namespace Components.GameResource
{
    [InternalBufferCapacity(ResourceType_Length.Value)]
    public struct ResourceWalletElement : IBufferElementData
    {
        public ResourceType Type;
        public uint Quantity;
    }

    public struct WalletChangedTag : IComponentData, IEnableableComponent
    {
    }

}

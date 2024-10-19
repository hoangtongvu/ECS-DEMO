using Core.GameResource;
using Unity.Entities;

namespace Components.GameResource
{
    public struct ResourceWalletElement : IBufferElementData
    {
        public ResourceType Type;
        public uint Quantity;
    }

    public struct WalletChangedTag : IComponentData, IEnableableComponent
    {
    }

}

using Core.GameResource;
using Unity.Entities;

namespace Components.GameResource
{
    public struct ResourceWalletElement : IBufferElementData
    {
        public ResourceType Type;
        public uint Quantity;
    }

    public struct WalletChanged : IComponentData
    {
        public bool Value;
    }

    // Note: Currently I don't know which Resource Type is deducted then below code is useless.
    public struct ResourceWalletChangedElement : IBufferElementData
    {
        public ResourceType Type;
    }

}

using ZBase.Foundation.PubSub;

namespace Core.GameResource
{
    public readonly record struct UpdateResourceQuantityMessage(ResourceType ResourceType, uint Quantity) : IMessage;
}
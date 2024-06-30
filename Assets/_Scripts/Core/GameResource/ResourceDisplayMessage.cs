using Core.GameResource;
using ZBase.Foundation.PubSub;

namespace Core.MyEvent.PubSub.Messages
{
    public readonly record struct ResourceDisplayMessage(ResourceType ResourceType, uint Quantity) : IMessage;

}
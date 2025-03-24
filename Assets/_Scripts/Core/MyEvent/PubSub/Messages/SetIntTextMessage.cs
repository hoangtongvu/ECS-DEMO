using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Core.MyEvent.PubSub.Messages
{
    public readonly record struct SetIntTextMessage(Entity SpawnerEntity, int SpawningProfileElementIndex, int Value) : IMessage;

}
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Core.MyEvent.PubSub.Messages
{
    public readonly record struct SetProgressBarMessage(Entity SpawnerEntity, int SpawningProfileElementIndex, float Value) : IMessage;

}
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Core.MyEvent.PubSub.Messages
{
    public readonly record struct SpawnUnitMessage(Entity SpawnerEntity, int SpawningProfileElementIndex) : IMessage;

}
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Core.Unit.Misc
{
    [System.Serializable]
	public readonly record struct DestroyEntityMessage(Entity BaseEntity) : IMessage;
}
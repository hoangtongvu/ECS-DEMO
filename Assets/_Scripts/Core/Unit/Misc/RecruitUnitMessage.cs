using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Core.Unit.Misc
{
    [System.Serializable]
	public readonly record struct RecruitUnitMessage(Entity BaseEntity) : IMessage;
}
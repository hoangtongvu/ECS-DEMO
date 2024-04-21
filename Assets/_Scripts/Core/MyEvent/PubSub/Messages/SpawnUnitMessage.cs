using Core.UI.Identification;
using ZBase.Foundation.PubSub;

namespace Core.MyEvent.PubSub.Messages
{
    public readonly record struct SpawnUnitMessage(UIID ProfileID) : IMessage;

}
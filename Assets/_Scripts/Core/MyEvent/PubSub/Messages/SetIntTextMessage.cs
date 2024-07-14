using Core.UI.Identification;
using ZBase.Foundation.PubSub;

namespace Core.MyEvent.PubSub.Messages
{
    public readonly record struct SetIntTextMessage(UIID UIID, int Value) : IMessage;

}
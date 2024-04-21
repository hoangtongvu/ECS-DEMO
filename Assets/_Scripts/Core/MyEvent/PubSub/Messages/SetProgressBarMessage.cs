using Core.UI.Identification;
using ZBase.Foundation.PubSub;

namespace Core.MyEvent.PubSub.Messages
{
    public readonly record struct SetProgressBarMessage(UIID UIID, float Value) : IMessage;

}
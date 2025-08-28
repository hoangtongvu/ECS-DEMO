using ZBase.Foundation.PubSub;

namespace Core.MyEvent.PubSub.Messages.WorldBuilding
{
    public readonly record struct ToggleBuildModeMessage(bool VisibleState) : IMessage;
}
using Core.MyEvent.PubSub.Messages.WorldBuilding;
using Core.MyEvent.PubSub.Messengers;
using ZBase.Foundation.PubSub;

namespace Core.UI.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton
{
    public class ExitButton : BaseButton
    {
        protected override void OnClick()
        {
            GameplayMessenger.MessagePublisher
                .Publish(new ToggleBuildModeMessage(false));
        }
    }
}
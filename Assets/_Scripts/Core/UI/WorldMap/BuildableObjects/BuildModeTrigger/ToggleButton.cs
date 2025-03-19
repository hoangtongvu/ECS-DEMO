using Core.MyEvent.PubSub.Messages.UI;
using Core.MyEvent.PubSub.Messengers;
using ZBase.Foundation.PubSub;

namespace Core.UI.WorldMap.BuildableObjects.BuildModeTrigger
{
    public class ToggleButton : BaseButton
    {
        protected override void OnClick()
        {
            GameplayMessenger.MessagePublisher
                .Publish<BuildModeToggleMessage>();

        }

    }

}
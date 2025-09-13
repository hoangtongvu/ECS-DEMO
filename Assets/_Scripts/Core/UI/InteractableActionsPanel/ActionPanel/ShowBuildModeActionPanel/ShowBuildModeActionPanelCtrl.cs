using Core.MyEvent.PubSub.Messages.WorldBuilding;
using Core.MyEvent.PubSub.Messengers;
using ZBase.Foundation.PubSub;

namespace Core.UI.InteractableActionsPanel.ActionPanel.ShowBuildModeActionPanel
{
    [GenerateUIType("ActionPanel_ShowBuildMode")]
    public partial class ShowBuildModeActionPanelCtrl : ActionPanelCtrl
    {
        public override void Activate()
        {
            base.Activate();
            GameplayMessenger.MessagePublisher
                .Publish(new ToggleBuildModeMessage(true));
        }

        public override void OnRent()
        {
        }

        public override void OnReturn()
        {
        }

    }

}
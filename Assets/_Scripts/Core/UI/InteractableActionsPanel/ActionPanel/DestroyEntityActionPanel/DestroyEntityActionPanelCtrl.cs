using Core.MyEvent.PubSub.Messengers;
using Core.Unit.Misc;
using ZBase.Foundation.PubSub;

namespace Core.UI.InteractableActionsPanel.ActionPanel.DestroyEntityActionPanel
{
    [GenerateUIType("ActionPanel_DestroyEntity")]
    public partial class DestroyEntityActionPanelCtrl : ActionPanelCtrl
    {
        public override void Activate()
        {
            base.Activate();
            GameplayMessenger.MessagePublisher
                .Publish(new DestroyEntityMessage(this.BaseEntity));
        }

        public override void OnRent()
        {
        }

        public override void OnReturn()
        {
        }

    }

}
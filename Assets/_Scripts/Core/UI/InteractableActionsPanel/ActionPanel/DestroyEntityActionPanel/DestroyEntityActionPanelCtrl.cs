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
            GameplayMessenger.MessagePublisher
                .Publish(new DestroyEntityMessage(this.BaseEntity));
        }

    }

}
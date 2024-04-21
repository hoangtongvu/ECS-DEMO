using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.UI.StructurePanel.UnitProfile
{
    public class UnitProfileUIButton : BaseButton
    {
        [SerializeField]private UnitProfileUICtrl unitProfileUICtrl;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(ref this.unitProfileUICtrl);
        }

        protected override void OnClick()
        {
            GameplayMessenger.MessagePublisher
                .Publish(new SpawnUnitMessage(this.unitProfileUICtrl.UIID));
            
        }

    }
}
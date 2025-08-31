using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Core.UI.TextMeshProUGUIs;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.UI.TopLeftPanel.ResourceDisplay
{
    public class QuantityText : BaseTextMeshProUGUI
    {
        [SerializeField] private ResourceDisplayCtrl resourceDisplayCtrl;
        private ISubscription subscription;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(ref this.resourceDisplayCtrl);
        }

        private void OnEnable()
        {
            this.subscription = GameplayMessenger.MessageSubscriber
                .Subscribe<ResourceDisplayMessage>(this.SetQuantity);
        }

        private void OnDisable() => this.subscription.Unsubscribe();

        private void SetQuantity(ResourceDisplayMessage message)
        {
            if (this.resourceDisplayCtrl.ResourceType != message.ResourceType) return;
            this.SetQuantity(message.Quantity);
        }

        public void SetQuantity(uint quantity) => this.text.text = $"{quantity}";
    }
}
using Core.GameResource;
using Core.MyEvent.PubSub.Messengers;
using System;
using ZBase.Foundation.PubSub;

namespace Core.UI.TopLeftPanel.ResourceDisplay
{
    public class ResourceDisplayEventHandler
    {
        private ISubscription updateResourceQuantityMessageSubscription;

        public ResourceDisplayCtrl ResourceDisplayCtrl { get; set; }
        public event Action OnQuantityChanged;
        public event Action OnQuantityAdded;
        public event Action OnQuantityDeducted;

        public void Bind()
        {
            this.updateResourceQuantityMessageSubscription = GameplayMessenger.MessageSubscriber
                .Subscribe<UpdateResourceQuantityMessage>(message =>
                {
                    var data = this.ResourceDisplayCtrl.ResourceDisplayData;
                    if (data.ResourceType != message.ResourceType) return;
                    if (data.ResourceQuantity == message.Quantity) return;

                    int deltaQuantity = (int)(data.ResourceQuantity - message.Quantity);
                    data.ResourceQuantity = message.Quantity;

                    if (deltaQuantity > 0)
                        OnQuantityDeducted?.Invoke();
                    else
                        OnQuantityAdded?.Invoke();

                    OnQuantityChanged?.Invoke();
                });
        }

        public void UnBind()
        {
            this.updateResourceQuantityMessageSubscription.Unsubscribe();
        }
    }
}
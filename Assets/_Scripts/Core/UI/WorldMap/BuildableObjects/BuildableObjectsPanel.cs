using Core.MyEvent.PubSub.Messages.UI;
using Core.MyEvent.PubSub.Messengers;
using ZBase.Foundation.PubSub;

namespace Core.UI.WorldMap.BuildableObjects
{
    [GenerateUIType("BuildableObjectsPanel")]
    public partial class BuildableObjectsPanel : BaseUICtrl
    {
        private ISubscription subscription;

        private void Start()
        {
            this.gameObject.SetActive(false);
            this.subscription = GameplayMessenger.MessageSubscriber
                .Subscribe<BuildModeToggleMessage>(this.HandleMessage);

        }

        private void OnDestroy() => this.subscription.Dispose();

        private void HandleMessage(BuildModeToggleMessage message)
        {
            this.gameObject.SetActive(!this.gameObject.activeSelf);
        }

    }

}
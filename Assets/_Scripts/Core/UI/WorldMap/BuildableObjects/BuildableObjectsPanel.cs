using Core.MyEvent.PubSub.Messages.UI;
using Core.MyEvent.PubSub.Messengers;
using ZBase.Foundation.PubSub;

namespace Core.UI.WorldMap.BuildableObjects
{
    [GenerateUIType("BuildableObjectsPanel")]
    public partial class BuildableObjectsPanel : BaseUICtrl
    {
        private void Start()
        {
            this.gameObject.SetActive(false);
            GameplayMessenger.MessageSubscriber
                .Subscribe<BuildModeToggleMessage>(this.HandleMessage);

        }

        void HandleMessage(BuildModeToggleMessage message)
        {
            this.gameObject.SetActive(!this.gameObject.activeSelf);
        }

    }

}
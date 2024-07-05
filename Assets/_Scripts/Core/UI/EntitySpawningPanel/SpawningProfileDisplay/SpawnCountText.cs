using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;
using Core.UI.LegacyText;

namespace Core.UI.EntitySpawningPanel.SpawningProfileDisplay
{
    public class SpawnCountText : BaseText
    {
        [SerializeField] private SpawningProfileDisplayCtrl spawningProfileDisplayCtrl;
        private ISubscription subscriptions;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(ref this.spawningProfileDisplayCtrl);
        }

        private void OnEnable()
        {
            this.subscriptions = GameplayMessenger.MessageSubscriber
                .Subscribe<SetIntTextMessage>(this.Handle);
        }

        private void OnDisable() => this.subscriptions.Unsubscribe();

        private void Handle(SetIntTextMessage message)
        {
            if (!message.UIID.Equals(this.spawningProfileDisplayCtrl.UIID)) return;
            if (message.Value < 0) Debug.LogWarning($"SpawnCount value is greater than 1, is potentially a bug");
            this.SetSpawnCount(message.Value);
        }

        public void SetSpawnCount(int value) => this.text.text = value.ToString();

    }
}
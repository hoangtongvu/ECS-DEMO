using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;
using Core.UI.TextMeshProUGUIs;

namespace Core.UI.InteractableActionsPanel.ActionPanel.EntitySpawningProfileActionPanel
{
    public class SpawnCountText : BaseTextMeshProUGUI
    {
        [SerializeField] private EntitySpawningProfileActionPanelCtrl ctrl;
        private ISubscription subscriptions;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(ref this.ctrl);
        }

        private void OnEnable()
        {
            this.subscriptions = GameplayMessenger.MessageSubscriber
                .Subscribe<SetIntTextMessage>(this.Handle);
        }

        private void OnDisable() => this.subscriptions.Unsubscribe();

        private void Handle(SetIntTextMessage message)
        {
            if (!message.SpawnerEntity.Equals(this.ctrl.BaseEntity)) return;
            if (message.SpawningProfileElementIndex != this.ctrl.SpawningProfileElementIndex) return;

            if (message.Value < 0) Debug.LogWarning($"SpawnCount value is greater than 1, is potentially a bug");
            this.SetSpawnCount(message.Value);
        }

        public void SetSpawnCount(int value) => this.text.text = $"{value}";

    }

}
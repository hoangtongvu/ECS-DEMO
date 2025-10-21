using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Core.UI.TextMeshProUGUIs;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.UI.InteractableActionsPanel.ActionPanel.EntitySpawningProfileActionPanel
{
    public class SpawnCountText : BaseTextMeshProUGUI
    {
        [SerializeField] private EntitySpawningProfileActionPanelCtrl ctrl;
        private ISubscription subscriptions;
        private int internalSpawnCount;

        private void Awake() => this.UpdateUI();

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(out this.ctrl);
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

            this.TrySetSpawnCount(message.Value);
        }

        public void TrySetSpawnCount(int newValue)
        {
            if (newValue == this.internalSpawnCount) return;

            this.internalSpawnCount = newValue;
            this.UpdateUI();
        }

        private void UpdateUI() => this.text.text = $"{this.internalSpawnCount}";

    }

}
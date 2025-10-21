using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Core.UI.MyImage;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.PubSub;
using Core.Utilities.Extensions;

namespace Core.UI.InteractableActionsPanel.ActionPanel.EntitySpawningProfileActionPanel
{
    public class SpawningProgressBar : ProgressBarByImage
    {
        [SerializeField] private EntitySpawningProfileActionPanelCtrl ctrl;
        private List<ISubscription> subscriptions = new();

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(out this.ctrl);
        }

        private void OnEnable()
        {
            GameplayMessenger.MessageSubscriber
                .Subscribe<SetProgressBarMessage>(this.Handle).AddTo(this.subscriptions);
        }

        private void OnDisable()
        {
            for (int i = 0; i < this.subscriptions.Count; i++)
            {
                this.subscriptions[i]?.Unsubscribe();
            }

            this.subscriptions.Clear();
        }

        private void Handle(SetProgressBarMessage message)
        {
            if (!message.SpawnerEntity.Equals(this.ctrl.BaseEntity)) return;
            if (message.SpawningProfileElementIndex != this.ctrl.SpawningProfileElementIndex) return;

            if (message.Value > 1) Debug.LogWarning($"Progress value is greater than 1, is potentially a bug");
            this.SetProgress(message.Value);
        }

    }

}
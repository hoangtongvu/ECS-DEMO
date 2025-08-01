using Core.Misc;
using Core.Misc.Presenter;
using JSAM;
using System;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.GameEntity.Presenter
{
    public abstract class SFXPlayerOnMessageInvoked<TMessage, TEnum> : SaiMonoBehaviour
        where TMessage : IMessage
        where TEnum : Enum
    {
        private ISubscription subscription;
        [SerializeField] private BasePresenter basePresenter;
        [SerializeField] private TEnum sfx;

        protected override void Awake()
        {
            this.LoadCtrl(ref this.basePresenter);
        }

        private void OnEnable()
        {
            subscription = basePresenter.Messenger.MessageSubscriber
                .Subscribe((TMessage message) =>
                {
                    AudioManager.PlaySound(this.sfx, this.transform);
                });
        }

        private void OnDisable()
        {
            subscription.Dispose();
        }

    }

}
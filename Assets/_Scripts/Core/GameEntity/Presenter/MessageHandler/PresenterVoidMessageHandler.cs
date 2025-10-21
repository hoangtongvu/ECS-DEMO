using Core.GameEntity.Presenter.MessageHandler.PresentingActions;
using Core.Misc;
using Core.Misc.Presenter;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.GameEntity.Presenter.MessageHandler
{
    public abstract class PresenterVoidMessageHandler<TMessage> : SaiMonoBehaviour
        where TMessage : IMessage
    {
        private ISubscription subscription;
        [SerializeField] private BasePresenter basePresenter;

        [SerializeReference, SubclassSelector]
        private List<PresentingAction> presentingActions;

        private void Awake()
        {
            this.LoadCtrl(out this.basePresenter);
            this.presentingActions.ForEach(action => action.Initialize(basePresenter, gameObject));
        }

        private void OnEnable()
        {
            this.subscription = this.basePresenter.Messenger.MessageSubscriber
                .Subscribe((TMessage message) =>
                {
                    this.presentingActions.ForEach(action => action.Activate(this.basePresenter, this.gameObject));
                });
        }

        private void OnDisable()
        {
            this.subscription.Dispose();
            this.presentingActions.ForEach(action => action.Dispose(basePresenter, gameObject));
        }

    }

}
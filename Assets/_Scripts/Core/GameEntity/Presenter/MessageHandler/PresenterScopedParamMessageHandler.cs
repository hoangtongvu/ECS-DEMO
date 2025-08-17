using Core.GameEntity.Presenter.MessageHandler.PresentingActions;
using Core.Misc;
using Core.Misc.Presenter;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.GameEntity.Presenter.MessageHandler
{
    public abstract class PresenterScopedParamMessageHandler<TMessage, TScope, TMessageAction> : SaiMonoBehaviour
        where TMessage : IMessage
        where TMessageAction : PresentingAction<TMessage>
    {
        private ISubscription subscription;
        [SerializeField] private BasePresenter basePresenter;
        [SerializeField] private TScope scope;

        [SerializeReference, SubclassSelector]
        private List<TMessageAction> paramPresentingActions;

        [SerializeReference, SubclassSelector]
        private List<PresentingAction> voidPresentingActions;
        
        protected override void Awake()
        {
            this.LoadCtrl(ref basePresenter);
            this.voidPresentingActions.ForEach(action => action.Initialize(basePresenter, gameObject));
            this.paramPresentingActions.ForEach(action => action.Initialize(basePresenter, gameObject));
        }

        private void OnEnable()
        {
            this.subscription = this.basePresenter.Messenger.MessageSubscriber
                .Scope(this.scope)
                .Subscribe((TMessage message) =>
                {
                    this.voidPresentingActions.ForEach(action => action.Activate(this.basePresenter, this.gameObject));
                    this.paramPresentingActions.ForEach(action => action.Activate(this.basePresenter, this.gameObject, message));
                });
        }

        private void OnDisable()
        {
            this.subscription.Dispose();
            this.voidPresentingActions.ForEach(action => action.Dispose(basePresenter, gameObject));
            this.paramPresentingActions.ForEach(action => action.Dispose(basePresenter, gameObject));
        }

    }

}
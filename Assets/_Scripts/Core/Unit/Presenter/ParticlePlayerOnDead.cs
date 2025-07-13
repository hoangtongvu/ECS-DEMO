using Core.Misc;
using Core.Misc.Presenter;
using Core.Misc.Presenter.PresenterMessages;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.Unit.Presenter
{
    public class ParticlePlayerOnDead : SaiMonoBehaviour
    {
        private ISubscription subscription;
        [SerializeField] private BasePresenter basePresenter;
        [SerializeField] private ParticleSystem deadParticle;

        protected override void Awake()
        {
            this.deadParticle.Stop();
            this.LoadCtrl(ref this.basePresenter);
        }

        private void OnEnable()
        {
            this.subscription = this.basePresenter.Messenger.MessageSubscriber
                .Subscribe<OnDeadMessage>((message) =>
                {
                    Instantiate(this.deadParticle, transform);
                });
        }

        private void OnDisable()
        {
            this.subscription.Dispose();
        }

    }

}
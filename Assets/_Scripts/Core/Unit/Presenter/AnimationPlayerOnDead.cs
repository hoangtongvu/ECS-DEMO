using Core.Animator;
using Core.Misc;
using Core.Misc.Presenter;
using Core.Misc.Presenter.PresenterMessages;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.Unit.Presenter
{
    public class AnimationPlayerOnDead : SaiMonoBehaviour
    {
        private ISubscription subscription;
        [SerializeField] private BasePresenter basePresenter;
        [SerializeField] private BaseAnimator baseAnimator;

        protected override void Awake()
        {
            this.LoadCtrl(ref this.basePresenter);
            this.LoadBaseAnimator();
        }

        private void LoadBaseAnimator()
        {
            this.baseAnimator = this.basePresenter.GetComponent<BaseAnimator>();
        }

        private void OnEnable()
        {
            this.subscription = this.basePresenter.Messenger.MessageSubscriber
                .Subscribe<OnDeadMessage>((message) =>
                {
                    this.baseAnimator.WaitPlay("Idle", 0.2f);
                });
        }

        private void OnDisable()
        {
            this.subscription.Dispose();
        }

    }

}
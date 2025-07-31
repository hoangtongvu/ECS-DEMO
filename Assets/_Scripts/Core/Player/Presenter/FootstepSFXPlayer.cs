using Audio.JSAM;
using Core.Animator.AnimationEvents;
using Core.Misc;
using Core.Misc.Presenter;
using JSAM;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.Player.Presenter
{
    public class FootstepSFXPlayer : SaiMonoBehaviour
    {
        private ISubscription subscription;
        [SerializeField] private BasePresenter basePresenter;
        [SerializeField] private PlayerSound_LibrarySounds sfx;
        [SerializeField] private AnimationEventChannel animationEventChannel;

        protected override void Awake()
        {
            this.LoadCtrl(ref this.basePresenter);
        }

        private void OnEnable()
        {
            this.subscription = this.basePresenter.Messenger.MessageSubscriber
                .Scope(this.animationEventChannel)
                .Subscribe<AnimationEventMessage>(this.PlaySFX);
        }

        private void OnDisable() => this.subscription.Dispose();

        private void PlaySFX() => AudioManager.PlaySound(this.sfx);

    }

}
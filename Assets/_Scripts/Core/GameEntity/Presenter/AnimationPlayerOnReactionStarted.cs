using Core.Animator;
using Core.Misc;
using Core.Misc.Presenter;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.GameEntity.Presenter
{
    public abstract class AnimationPlayerOnReactionStarted<TMessage> : SaiMonoBehaviour
        where TMessage : IMessage
    {
        private ISubscription subscription;
        [SerializeField] private BasePresenter basePresenter;
        [SerializeField] private BaseAnimator baseAnimator;
        [SerializeField] private string animationName;
        [SerializeField] private float transitionDuration = 0.2f;

        protected override void Awake()
        {
            LoadCtrl(ref basePresenter);
            LoadBaseAnimator();
        }

        private void LoadBaseAnimator()
        {
            baseAnimator = basePresenter.GetComponent<BaseAnimator>();
        }

        private void OnEnable()
        {
            subscription = basePresenter.Messenger.MessageSubscriber
                .Subscribe((TMessage message) =>
                {
                    baseAnimator.WaitPlay(animationName, transitionDuration);
                });
        }

        private void OnDisable()
        {
            subscription.Dispose();
        }

    }

}
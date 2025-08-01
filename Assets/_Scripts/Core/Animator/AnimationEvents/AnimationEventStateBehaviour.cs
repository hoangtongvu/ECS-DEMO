using Core.Misc.Presenter;
using System;
using UnityEngine;

namespace Core.Animator.AnimationEvents
{
    public class AnimationEventStateBehaviour : StateMachineBehaviour
    {
        [SerializeField] private AnimationEventChannel animationEventChannel;
        [SerializeField] private int lastLoopCount;

        [Range(0f, 1f)]
        [SerializeField]
        private float triggerTime;

        public override void OnStateEnter(UnityEngine.Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            this.lastLoopCount = -1;
        }

        public override void OnStateUpdate(UnityEngine.Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float rawTime = stateInfo.normalizedTime;
            int loopCount = Mathf.FloorToInt(rawTime);
            float currentTime = rawTime % 1f;

            if (loopCount != this.lastLoopCount && currentTime >= this.triggerTime)
            {
                this.lastLoopCount = loopCount;
                this.PublishEventMessage(animator);
            }
        }

        private void PublishEventMessage(UnityEngine.Animator animator)
        {
            if (!animator.TryGetComponent<BasePresenter>(out var basePresenter))
            {
                UnityEngine.Debug.LogWarning($"{nameof(AnimationEventStateBehaviour)} can't find {nameof(BasePresenter)} on the same GameObject as {nameof(UnityEngine.Animator)}");
                return;
            }

            basePresenter.Messenger.MessagePublisher
                .Scope(this.animationEventChannel)
                .Publish<AnimationEventMessage>();
            
        }

    }

}
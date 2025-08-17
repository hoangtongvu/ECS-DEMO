using Core.Animator;
using Core.Misc.Presenter;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions
{
    [Serializable]
    public class PlayAnimationAction : PresentingAction
    {
        [SerializeField] private BaseAnimator baseAnimator;
        [SerializeField] private string animationName;
        [SerializeField] private float transitionDuration = 0.2f;

        public override void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            baseAnimator = basePresenter.GetComponent<BaseAnimator>();
        }

        public override void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            baseAnimator.WaitPlay(animationName, transitionDuration);
        }

        public override void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }
    }
}
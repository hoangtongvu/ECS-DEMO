using Core.Animator.AnimationEvents;
using Core.Misc.Presenter;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Core.Player.Presenter.MessageHandler.PresentingActions
{
    [Serializable]
    public class DisableWeaponTrailEmittingAction : AnimationEventAction
    {
        [SerializeField] private int messageId;
        [SerializeField] private TrailRenderer target;

        public override void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }

        public override void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj, [NotNull] AnimationEventMessage message)
        {
            if (message.Id != this.messageId) return;
            this.target.emitting = false;
        }

        public override void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }
    }
}
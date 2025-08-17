using Core.Misc.Presenter;
using LitMotion;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions
{
    [Serializable]
    public class RotateOnDeadAction : PresentingAction
    {
        private MotionHandle motionHandle;

        public override void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }

        public override void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            var targetRotation = basePresenter.transform.rotation * Quaternion.AngleAxis(90, Vector3.forward);

            this.motionHandle = LMotion.Create(basePresenter.transform.rotation, targetRotation, 1f)
                .WithEase(Ease.OutExpo)
                .Bind(tempRotation => basePresenter.transform.rotation = tempRotation);
        }

        public override void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            this.motionHandle.TryCancel();
        }
    }
}
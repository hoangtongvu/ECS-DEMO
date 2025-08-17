using Core.Misc.Presenter;
using LitMotion;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions
{
    [Serializable]
    public class MoveToGroundAction : PresentingAction
    {
        private MotionHandle motionHandle;
        [SerializeField] private float motionDelaySeconds = 1f;
        [SerializeField] private float motionDurationSeconds = 1f;
        [SerializeField] private float deltaY = 5f;

        public override void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }

        public override void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            Vector3 targetPos = basePresenter.transform.position;
            targetPos.y -= deltaY;

            this.motionHandle = LMotion.Create(basePresenter.transform.position, targetPos, motionDurationSeconds)
                .WithDelay(motionDelaySeconds)
                .WithEase(Ease.Linear)
                .Bind(tempPos => basePresenter.transform.position = tempPos);
        }

        public override void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            this.motionHandle.TryCancel();
        }
    }
}
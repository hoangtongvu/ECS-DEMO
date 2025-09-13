using LitMotion;
using System;
using UnityEngine;

namespace Core.UI.InteractableActionsPanel;

[Serializable]
public class ActionsContainerUITweener
{
    public ActionsContainerUICtrl ctrl;

    [Header("Tween")]
    private MotionHandle motionHandle;
    [SerializeField] private float startAlpha = 0;
    [SerializeField] private float toAlpha = 1;
    [SerializeField] private float tweenDurationSeconds = 0.5f;
    [SerializeField] private Ease easeType;

    public void TriggerTweenOnAppear()
    {
        this.motionHandle.TryCancel();

        float startAlpha = this.ctrl.CanvasGroup.alpha;
        float endAlpha = this.toAlpha;

        this.motionHandle = LMotion.Create(startAlpha, endAlpha, this.tweenDurationSeconds)
            .WithEase(this.easeType)
            .Bind(tempAlpha => this.ctrl.CanvasGroup.alpha = tempAlpha);
    }

    public void TriggerTweenOnDisappear()
    {
        this.motionHandle.TryCancel();

        float startAlpha = this.ctrl.CanvasGroup.alpha;
        float endAlpha = this.startAlpha;

        this.motionHandle = LMotion.Create(startAlpha, endAlpha, this.tweenDurationSeconds)
            .WithEase(this.easeType)
            .WithOnComplete(() =>
            {
                this.ctrl.ReturnSelfToPool();
            })
            .Bind(tempA => this.ctrl.CanvasGroup.alpha = tempA);
    }

}
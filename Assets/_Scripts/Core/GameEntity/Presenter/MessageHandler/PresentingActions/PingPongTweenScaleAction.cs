using Core.Misc.Presenter;
using LitMotion;
using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using UnityEngine;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions;

[Serializable]
public class PingPongTweenScaleAction : PresentingAction
{
    private float3 originalScale;
    [SerializeField] private Transform target;

    [Header("Tween Data")]
    private MotionHandle motionHandle;
    [SerializeField] private float3 peakScaleMultiplier;
    [SerializeField] private float forwardDuration;
    [SerializeField] private Ease forwardEase;
    [SerializeField] private float backwardDuration;
    [SerializeField] private Ease backwardEase;

    public override void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
    {
        this.originalScale = this.target.localScale;
    }

    public override void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
    {
        this.motionHandle.TryCancel();

        this.motionHandle = LSequence.Create()
            .Append(this.GetForwardMotion())
            .Append(this.GetBackwardMotion())
            .Run();
    }

    public override void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
    {
        this.motionHandle.TryCancel();
    }

    private MotionHandle GetForwardMotion()
    {
        float3 startValue = this.target.localScale;
        float3 endValue = this.peakScaleMultiplier * this.originalScale;

        return LMotion.Create(startValue, endValue, this.forwardDuration)
            .WithEase(this.forwardEase)
            .Bind(tempScale => this.target.localScale = tempScale);
    }

    private MotionHandle GetBackwardMotion()
    {
        float3 startValue = this.peakScaleMultiplier * this.originalScale;
        float3 endValue = this.originalScale;

        return LMotion.Create(startValue, endValue, this.backwardDuration)
            .WithEase(this.backwardEase)
            .Bind(tempScale => this.target.localScale = tempScale);
    }
}
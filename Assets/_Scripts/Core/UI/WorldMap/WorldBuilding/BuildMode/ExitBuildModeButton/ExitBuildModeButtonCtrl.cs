using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton
{
    [GenerateUIType("ExitBuildModeButton")]
    public partial class ExitBuildModeButtonCtrl : BaseUICtrl, IReusableUI
    {
        [SerializeField] private Image image;

        [Header("Tween")]
        private MotionHandle motionHandle;
        [SerializeField] private float originalImageAlpha;
        [SerializeField] private float tweenDurationSeconds = 0.5f;
        [SerializeField] private Ease easeType;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(out this.image);
            this.LoadOriginalImageAlpha();
        }

        private void LoadOriginalImageAlpha()
        {
            this.originalImageAlpha = this.image.color.a;
        }

        public void Reuse() => this.TriggerTweenOnAppear();

        public override void TriggerHiding() => this.TriggerTweenOnDisappear();

        public override void OnRent()
        {
            this.SetImageAlpha(0);
            this.TriggerTweenOnAppear();
        }

        public override void OnReturn()
        {
            this.motionHandle.TryCancel();
            this.SetImageAlpha(this.originalImageAlpha);
        }

        private void SetImageAlpha(float alphaValue)
        {
            Color color = this.image.color;
            color.a = alphaValue;
            this.image.color = color;
        }

        private void TriggerTweenOnAppear()
        {
            this.motionHandle.TryCancel();
            float startValue = this.image.color.a;
            float endValue = this.originalImageAlpha;

            this.motionHandle = LMotion.Create(startValue, endValue, this.tweenDurationSeconds)
                .WithEase(this.easeType)
                .Bind(tempAlpha => this.SetImageAlpha(tempAlpha));
        }

        private void TriggerTweenOnDisappear()
        {
            this.motionHandle.TryCancel();
            float startValue = this.image.color.a;
            float endValue = 0f;

            this.motionHandle = LMotion.Create(startValue, endValue, this.tweenDurationSeconds)
                .WithEase(this.easeType)
                .WithOnComplete(() =>
                {
                    this.ReturnSelfToPool();
                })
                .Bind(tempAlpha => this.SetImageAlpha(tempAlpha));
        }
    }
}
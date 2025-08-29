using LitMotion;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton
{
    [GenerateUIType("ExitBuildModeButton")]
    public partial class ExitBuildModeButtonCtrl : BaseUICtrl
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
            this.LoadComponentInCtrl(ref this.image);
            this.LoadOriginalImageAlpha();
        }

        private void LoadOriginalImageAlpha()
        {
            this.originalImageAlpha = this.image.color.a;
        }

        public override void OnRent()
        {
            this.motionHandle = LMotion.Create(0, this.originalImageAlpha, this.tweenDurationSeconds)
                .WithEase(this.easeType)
                .Bind(tempAlpha => this.SetImageAlpha(tempAlpha));
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
    }
}
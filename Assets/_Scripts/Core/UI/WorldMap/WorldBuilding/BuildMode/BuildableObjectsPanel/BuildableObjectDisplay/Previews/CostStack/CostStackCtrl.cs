using Core.Utilities.Extensions;
using LitMotion;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel.BuildableObjectDisplay.Previews.CostStack
{
    [GenerateUIType("CostStack")]
    public partial class CostStackCtrl : BaseUICtrl
    {
        [SerializeField] private Image image;
        public int ContainerLength;
        public int IndexInContainer;

        [Header("Tween")]
        private MotionHandle motionHandle;
        [SerializeField] private float tweenDuration;
        [SerializeField] private Ease onExpandedEase;
        [SerializeField] private Ease onCollapsedEase;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private float2 originalPos;

        public Image Image => image;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(ref this.image);
            this.LoadComponentInCtrl(ref this.rectTransform);
        }

        public override void OnRent()
        {
            this.rectTransform.anchoredPosition = this.originalPos;
        }

        public override void OnReturn()
        {
        }

        public void TriggerTweenOnExpanded()
        {
            this.motionHandle.TryCancel();

            float startY = this.rectTransform.anchoredPosition.y;
            float endY = 20 * (this.ContainerLength - this.IndexInContainer);

            this.motionHandle = LMotion.Create(startY, endY, this.tweenDuration)
                .WithEase(this.onExpandedEase)
                .Bind(tempY => this.SetY(tempY));
        }

        public void TriggerTweenOnCollapsed()
        {
            this.motionHandle.TryCancel();

            float startY = this.rectTransform.anchoredPosition.y;
            float endY = this.originalPos.y;

            this.motionHandle = LMotion.Create(startY, endY, this.tweenDuration)
                .WithEase(this.onCollapsedEase)
                .Bind(tempY => this.SetY(tempY));
        }

        private void SetY(float yValue)
        {
            this.rectTransform.anchoredPosition = this.rectTransform.anchoredPosition.With(y: yValue);
        }

    }

}

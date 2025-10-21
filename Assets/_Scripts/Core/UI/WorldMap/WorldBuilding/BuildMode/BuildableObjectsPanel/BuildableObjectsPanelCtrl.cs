using Core.Utilities.Extensions;
using LitMotion;
using UnityEngine;

namespace Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel
{
    [GenerateUIType("BuildableObjectsPanel")]
    public partial class BuildableObjectsPanelCtrl : BaseUICtrl, IReusableUI
    {
        [Header("Components")]
        [SerializeField] private RectTransform selfRectTransform;
        [SerializeField] private ObjectDisplaysHolder objectDisplaysHolder;

        [Header("Tween")]
        private MotionHandle motionHandle;
        [SerializeField] private float originalY;
        [SerializeField] private float startOffsetY = -3f;
        [SerializeField] private float toOffsetY = 3f;
        [SerializeField] private float tweenDurationSeconds = 0.5f;
        [SerializeField] private Ease easeType;

        public ObjectDisplaysHolder ObjectDisplaysHolder => objectDisplaysHolder;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(out this.selfRectTransform);
            this.LoadComponentInChildren(out this.objectDisplaysHolder);
            this.LoadOriginalY();
        }

        private void LoadOriginalY()
        {
            this.originalY = this.selfRectTransform.anchoredPosition.y;
        }

        public override void TriggerHiding() => this.TriggerPanelTweenOnDisappear();

        public void Reuse()
        {
            this.ClearChildren();
            this.TriggerPanelTweenOnAppear();
        }

        public override void OnRent()
        {
            this.SetPanelY(this.originalY + this.startOffsetY);
            this.TriggerPanelTweenOnAppear();
        }

        public override void OnReturn() => this.ClearChildren();

        private void SetPanelY(float yValue)
        {
            this.selfRectTransform.anchoredPosition =
                this.selfRectTransform.anchoredPosition.With(y: yValue);
        }

        private void TriggerPanelTweenOnAppear()
        {
            this.motionHandle.TryCancel();

            float startY = this.selfRectTransform.anchoredPosition.y;
            float endY = this.originalY + this.toOffsetY;

            this.motionHandle = LMotion.Create(startY, endY, this.tweenDurationSeconds)
                .WithEase(this.easeType)
                .Bind(tempY => this.SetPanelY(tempY));
        }

        private void TriggerPanelTweenOnDisappear()
        {
            this.motionHandle.TryCancel();

            float startY = this.selfRectTransform.anchoredPosition.y;
            float endY = this.originalY + this.startOffsetY;

            this.motionHandle = LMotion.Create(startY, endY, this.tweenDurationSeconds)
                .WithEase(this.easeType)
                .WithOnComplete(() =>
                {
                    this.ReturnSelfToPool();
                })
                .Bind(tempY => this.SetPanelY(tempY));
        }

        private void ClearChildren()
        {
            foreach (var displayCtrl in this.objectDisplaysHolder.Displays)
            {
                displayCtrl.ReturnSelfToPool();
            }

            this.objectDisplaysHolder.Displays.Clear();
        }

    }

}
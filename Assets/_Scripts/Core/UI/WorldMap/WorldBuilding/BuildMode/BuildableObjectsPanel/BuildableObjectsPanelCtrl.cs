using Core.Utilities.Extensions;
using LitMotion;
using UnityEngine;

namespace Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel
{
    [GenerateUIType("BuildableObjectsPanel")]
    public partial class BuildableObjectsPanelCtrl : BaseUICtrl
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
            this.LoadComponentInCtrl(ref this.selfRectTransform);
            this.LoadComponentInChildren(ref this.objectDisplaysHolder);
            this.LoadOriginalY();
        }

        private void LoadOriginalY()
        {
            this.originalY = this.selfRectTransform.anchoredPosition.y;
        }

        public override void OnRent()
        {
            this.motionHandle = LMotion.Create(this.selfRectTransform.anchoredPosition.y, this.toOffsetY, this.tweenDurationSeconds)
                .WithEase(this.easeType)
                .Bind(tempOffsetY => selfRectTransform.anchoredPosition = selfRectTransform.anchoredPosition.With(y: originalY + tempOffsetY));
        }

        public override void OnReturn()
        {
            this.motionHandle.TryCancel();
            this.selfRectTransform.anchoredPosition = this.selfRectTransform.anchoredPosition.With(y: this.originalY + this.startOffsetY);

            foreach (var displayCtrl in this.objectDisplaysHolder.Displays)
            {
                displayCtrl.ReturnSelfToPool();
            }

            this.objectDisplaysHolder.Displays.Clear();
        }

    }

}
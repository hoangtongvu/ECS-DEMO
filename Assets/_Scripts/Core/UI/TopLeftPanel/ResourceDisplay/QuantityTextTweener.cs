using LitMotion;
using System;
using UnityEngine;

namespace Core.UI.TopLeftPanel.ResourceDisplay
{
    [Serializable]
    public class QuantityTextTweener
    {
        private MotionHandle motionHandle;

        public Color OriginalTextColor;
        public Color TextColorOnQuantityAdded = Color.green;
        public Color TextColorOnQuantityDeducted = Color.red;

        public float Duration = 0.3f;
        public Ease ForwardEase;
        public Ease BackwardEase;

        public QuantityText QuantityText { get; set; }

        public void TriggerOnQuantityAddedTweens()
        {
            this.motionHandle.TryCancel();

            this.motionHandle = LSequence.Create()
                .Append(this.GetCurrentToPeakColorTween(this.TextColorOnQuantityAdded))
                .Append(this.GetToOriginalColorTween(this.TextColorOnQuantityAdded))
                .Run();
        }

        public void TriggerOnQuantityDeductedTweens()
        {
            this.motionHandle.TryCancel();

            this.motionHandle = LSequence.Create()
                .Append(this.GetCurrentToPeakColorTween(this.TextColorOnQuantityDeducted))
                .Append(this.GetToOriginalColorTween(this.TextColorOnQuantityDeducted))
                .Run();
        }

        private MotionHandle GetCurrentToPeakColorTween(Color peakValue)
        {
            Color startValue = this.QuantityText.TextMeshProUGUI.color;

            return LMotion.Create(startValue, peakValue, this.Duration)
                .WithEase(this.ForwardEase)
                .Bind(tempValue => this.ChangeTextColor(tempValue));
        }

        private MotionHandle GetToOriginalColorTween(Color startValue)
        {
            Color toValue = this.OriginalTextColor;

            return LMotion.Create(startValue, toValue, this.Duration)
                .WithEase(this.BackwardEase)
                .Bind(tempValue => this.ChangeTextColor(tempValue));
        }

        public void ChangeTextColor(Color value) => this.QuantityText.TextMeshProUGUI.color = value;

    }

}
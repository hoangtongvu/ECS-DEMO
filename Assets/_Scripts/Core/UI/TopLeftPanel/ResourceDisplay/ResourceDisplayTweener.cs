using LitMotion;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Core.UI.TopLeftPanel.ResourceDisplay
{
    [Serializable]
    public class ResourceDisplayTweener
    {
        private MotionHandle motionHandle;
        public float Duration = 0.3f;

        public float ShakeStrength;

        [field: SerializeField] public ResourceDisplayCtrl ResourceDisplayCtrl { get; set; }

        public void TriggerOnQuantityChangedTweens()
        {
            this.motionHandle.TryComplete();

            this.motionHandle = LSequence.Create()
                .Append(this.GetShakeTween())
                .Run();
        }

        private MotionHandle GetShakeTween()
        {
            float3 startValue = this.ResourceDisplayCtrl.transform.position;

            return LMotion.Shake.Create(startValue, new float3(this.ShakeStrength), this.Duration)
                .Bind(tempValue => this.ResourceDisplayCtrl.transform.position = tempValue);
        }

    }

}
using LitMotion;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Misc
{
    public class TransformScaler : MonoBehaviour
    {
        [SerializeField] private float tweenDuration = 0.4f;
        [SerializeField] private float3 targetScale = new Vector3(1.3f, 1.3f, 1f);
        [SerializeField] private float3 originalScale;

        private MotionHandle scaleUpMotionHandle;
        private MotionHandle scaleDownMotionHandle;

        private void Awake()
        {
            this.originalScale = transform.localScale;
        }

        private void OnDisable()
        {
            this.scaleUpMotionHandle.TryCancel();
            this.scaleDownMotionHandle.TryCancel();
            transform.localScale = this.originalScale;
        }

        public void ScaleUp()
        {
            this.scaleUpMotionHandle.TryCancel();
            this.scaleDownMotionHandle.TryCancel();

            this.scaleUpMotionHandle = LMotion.Create(transform.localScale, this.targetScale, tweenDuration)
                .WithEase(Ease.OutExpo)
                .Bind(tempScale => transform.localScale = tempScale);
        }

        public void ScaleDown()
        {
            this.scaleUpMotionHandle.TryCancel();
            this.scaleDownMotionHandle.TryCancel();

            this.scaleDownMotionHandle = LMotion.Create(transform.localScale, this.originalScale, tweenDuration)
                .WithEase(Ease.OutExpo)
                .Bind(tempScale => transform.localScale = tempScale);
        }

    }

}
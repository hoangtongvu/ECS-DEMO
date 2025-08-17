using Core.Misc.Presenter;
using Core.Misc.Presenter.PresenterMessages;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions
{
    [Serializable]
    public class SpawnFXOnHpLevelsAction : OnHitPresentingAction
    {
        [SerializeField] private GameObject currentFX;
        [SerializeField] private float currentThreshold = 1f;

        [Tooltip("The first element in the pool will spawn last")]
        [SerializeField]
        private GameObject[] fxPool;

        public override void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }

        public override void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj, [NotNull] OnHitMessage message)
        {
            int length = this.fxPool.Length;
            float stepSize = 1f / (length + 1);
            float hpRatio = message.remainingHpRatio;

            for (int i = length - 1; i >= 0; i--)
            {
                float upperThreshold = (i + 1) * stepSize;
                float lowerThreshold = i * stepSize;

                if (hpRatio > upperThreshold || hpRatio < lowerThreshold) continue;
                if (this.currentThreshold == upperThreshold) continue;

                this.currentThreshold = upperThreshold;
                if (this.currentFX != null) GameObject.Destroy(this.currentFX.gameObject);
                this.currentFX = GameObject.Instantiate(this.fxPool[length - 1 - i], baseGameObj.transform);

                break;
            }
        }

        public override void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }
    }
}
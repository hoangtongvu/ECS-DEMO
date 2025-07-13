using Core.Misc;
using Core.Misc.Presenter;
using Core.Misc.Presenter.PresenterMessages;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.GameEntity.Presenter
{
    public class FXSpawnerOnHpLevels : SaiMonoBehaviour
    {
        private ISubscription subscription;
        [SerializeField] private BasePresenter basePresenter;
        [SerializeField] private GameObject currentFX;
        [SerializeField] private float currentThreshold = 1f;

        [Tooltip("The first element in the pool will spawn last")]
        [SerializeField]
        private GameObject[] fxPool;

        protected override void Awake()
        {
            this.LoadCtrl(ref this.basePresenter);
        }

        private void OnEnable()
        {
            this.subscription = this.basePresenter.Messenger.MessageSubscriber
                .Subscribe<OnHitMessage>(this.HandleFX);
        }

        private void OnDisable()
        {
            this.subscription.Dispose();
        }

        private void HandleFX(OnHitMessage onHitMessage)
        {
            int length = this.fxPool.Length;
            float stepSize = 1f / (length + 1);
            float hpRatio = onHitMessage.remainingHpRatio;

            for (int i = length - 1; i >= 0; i--)
            {
                float upperThreshold = (i + 1) * stepSize;
                float lowerThreshold = i * stepSize;

                if (hpRatio > upperThreshold || hpRatio < lowerThreshold) continue;
                if (this.currentThreshold == upperThreshold) continue;

                this.currentThreshold = upperThreshold;
                if (this.currentFX != null) GameObject.Destroy(this.currentFX.gameObject);
                this.currentFX = Instantiate(this.fxPool[length - 1 - i], this.transform);

                break;

            }

        }

    }

}
using Core.GameBuilding.BuildingConstruction.Presenter.PresenterMessages;
using Core.Misc;
using Core.Misc.Presenter;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.GameBuilding.BuildingConstruction.Presenter
{
    public class FXSpawnOnEndConstructionHandler : SaiMonoBehaviour
    {
        private ISubscription subscription;
        [SerializeField] private BasePresenter basePresenter;
        [SerializeField] private GameObject fxPrefab;

        protected override void Awake()
        {
            this.LoadCtrl(ref this.basePresenter);
        }

        private void OnEnable()
        {
            this.subscription = this.basePresenter.Messenger.MessageSubscriber
                .Subscribe<OnConstructionEndedMessage>(this.HandleFX);
        }

        private void OnDisable()
        {
            this.subscription.Dispose();
        }

        private void HandleFX(OnConstructionEndedMessage onHitMessage)
        {
            Instantiate(this.fxPrefab, this.transform);
        }

    }

}
using Core.Misc;
using Core.Misc.Presenter;
using Core.Misc.Presenter.PresenterMessages;
using LitMotion;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.GameEntity.Presenter
{
    public class MoveToGroundOnDeadHandler : SaiMonoBehaviour
    {
        private ISubscription subscription;
        private MotionHandle motionHandle;
        [SerializeField] private BasePresenter basePresenter;
        [SerializeField] private float motionDelaySeconds = 1f;
        [SerializeField] private float motionDurationSeconds = 1f;
        [SerializeField] private float deltaY = 5f;

        protected override void Awake()
        {
            this.LoadCtrl(ref this.basePresenter);
        }

        private void OnEnable()
        {
            this.subscription = this.basePresenter.Messenger.MessageSubscriber
                .Subscribe<OnDeadMessage>((message) =>
                {
                    Vector3 targetPos = this.basePresenter.transform.position;
                    targetPos.y -= deltaY;

                    this.motionHandle = LMotion.Create(this.basePresenter.transform.position, targetPos, this.motionDurationSeconds)
                        .WithDelay(this.motionDelaySeconds)
                        .WithEase(Ease.Linear)
                        .Bind(tempPos => this.basePresenter.transform.position = tempPos);
                });
        }

        private void OnDisable()
        {
            this.subscription.Dispose();
            this.motionHandle.TryCancel();
        }

    }

}
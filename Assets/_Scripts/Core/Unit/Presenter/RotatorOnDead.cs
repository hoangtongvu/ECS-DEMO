using Core.Misc;
using Core.Misc.Presenter;
using Core.Misc.Presenter.PresenterMessages;
using LitMotion;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.Unit.Presenter
{
    public class RotatorOnDead : SaiMonoBehaviour
    {
        private ISubscription subscription;
        private MotionHandle motionHandle;
        [SerializeField] private BasePresenter basePresenter;

        protected override void Awake()
        {
            this.LoadCtrl(ref this.basePresenter);
        }

        private void OnEnable()
        {
            this.subscription = this.basePresenter.Messenger.MessageSubscriber
                .Subscribe<OnDeadMessage>((message) =>
                {
                    var targetRotation = this.basePresenter.transform.rotation * Quaternion.AngleAxis(90, Vector3.forward);

                    this.motionHandle = LMotion.Create(this.basePresenter.transform.rotation, targetRotation, 1f)
                        .WithEase(Ease.OutExpo)
                        .Bind(tempRotation => this.basePresenter.transform.rotation = tempRotation);
                });
        }

        private void OnDisable()
        {
            this.subscription.Dispose();
            this.motionHandle.TryCancel();
        }

    }

}
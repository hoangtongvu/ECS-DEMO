using Core.Misc.Presenter.PresenterMessages;
using ZBase.Foundation.PubSub;

namespace Core.Unit.Presenter
{
    public class RendererFlasherOnTakeHit : RendererFlasher
    {
        private ISubscription subscription;

        private void OnEnable()
        {
            this.subscription = this.basePresenter.Messenger.MessageSubscriber
                .Subscribe<OnHitMessage>(this.Flash);
        }

        private void OnDisable()
        {
            this.subscription.Dispose();
        }

    }

}
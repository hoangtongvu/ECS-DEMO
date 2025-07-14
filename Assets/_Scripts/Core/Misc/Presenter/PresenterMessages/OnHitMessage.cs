using ZBase.Foundation.PubSub;

namespace Core.Misc.Presenter.PresenterMessages
{
    public readonly record struct OnHitMessage(int hitDamageValue, float remainingHpRatio) : IMessage;
}
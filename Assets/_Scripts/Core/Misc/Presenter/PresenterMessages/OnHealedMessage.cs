using ZBase.Foundation.PubSub;

namespace Core.Misc.Presenter.PresenterMessages;

public readonly record struct OnHealedMessage(int HealValue, float RemainingHpRatio) : IMessage;
using Core.Misc.Presenter.PresenterMessages;
using System;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions;

[Serializable]
public abstract class OnHealedPresentingAction : PresentingAction<OnHealedMessage>
{
}
using Core.GameEntity.Presenter.MessageHandler.PresentingActions;
using System;

namespace Core.Animator.AnimationEvents
{
    [Serializable]
    public abstract class AnimationEventAction : PresentingAction<AnimationEventMessage>
    {
    }
}
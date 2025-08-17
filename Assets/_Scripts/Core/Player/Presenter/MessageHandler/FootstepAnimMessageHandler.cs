using Core.Animator.AnimationEvents;
using Core.GameEntity.Presenter.MessageHandler;

namespace Core.Player.Presenter.MessageHandler
{
    public class FootstepAnimMessageHandler
        : PresenterScopedParamMessageHandler<AnimationEventMessage, AnimationEventChannel, AnimationEventAction>
    {
    }
}
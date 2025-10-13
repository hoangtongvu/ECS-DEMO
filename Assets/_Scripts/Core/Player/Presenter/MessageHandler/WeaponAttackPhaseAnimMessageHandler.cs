using Core.Animator.AnimationEvents;
using Core.GameEntity.Presenter.MessageHandler;

namespace Core.Player.Presenter.MessageHandler
{
    public class WeaponAttackPhaseAnimMessageHandler
        : PresenterScopedParamMessageHandler<AnimationEventMessage, AnimationEventChannel, AnimationEventAction>
    {
    }
}
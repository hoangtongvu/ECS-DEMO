using Core.GameEntity.Presenter;
using Core.GameEntity.Reaction;

namespace Core.Player.Presenter
{
    public class AnimationPlayerOnIdleStarted : AnimationPlayerOnReactionStarted<OnIdleStartedMessage>
    {
        protected override string GetAnimName() => "Idle";

    }

}
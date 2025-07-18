using Core.GameEntity.Presenter;
using Core.GameEntity.Reaction;

namespace Core.Player.Presenter
{
    public class AnimationPlayerOnRunStarted : AnimationPlayerOnReactionStarted<OnRunStartedMessage>
    {
        protected override string GetAnimName() => "Running";

    }

}
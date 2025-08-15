using Core.GameEntity.Presenter;
using Core.GameEntity.Reaction;

namespace Core.Unit.Presenter
{
    public class AnimationPlayerOnRunStarted : AnimationPlayerOnReactionStarted<OnRunStartedMessage>
    {
        protected override string GetAnimName() => "Running_A";

    }

}
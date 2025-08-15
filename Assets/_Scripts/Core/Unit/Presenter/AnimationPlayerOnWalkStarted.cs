using Core.GameEntity.Presenter;
using Core.GameEntity.Reaction;

namespace Core.Unit.Presenter
{
    public class AnimationPlayerOnWalkStarted : AnimationPlayerOnReactionStarted<OnWalkStartedMessage>
    {
        protected override string GetAnimName() => "Walking_A";

    }

}
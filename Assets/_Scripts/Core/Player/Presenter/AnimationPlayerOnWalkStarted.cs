using Core.GameEntity.Presenter;
using Core.GameEntity.Reaction;

namespace Core.Player.Presenter
{
    public class AnimationPlayerOnWalkStarted : AnimationPlayerOnReactionStarted<OnWalkStartedMessage>
    {
        protected override string GetAnimName() => "Walking";

    }

}
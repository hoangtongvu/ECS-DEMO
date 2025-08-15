using Core.GameEntity.Presenter;
using Core.GameEntity.Reaction;

namespace Core.Unit.Presenter
{
    public class AnimationPlayerOnIdleStarted : AnimationPlayerOnReactionStarted<OnIdleStartedMessage>
    {
        protected override string GetAnimName() => "Idle";

    }

}
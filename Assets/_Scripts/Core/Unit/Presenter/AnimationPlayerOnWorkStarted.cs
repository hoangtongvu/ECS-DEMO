using Core.GameEntity.Presenter;
using Core.Unit.Reaction;

namespace Core.Unit.Presenter
{
    public class AnimationPlayerOnWorkStarted : AnimationPlayerOnReactionStarted<OnWorkStartedMessage>
    {
        protected override string GetAnimName() => "1H_Melee_Attack_Chop";

    }

}
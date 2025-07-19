using Core.GameEntity.Presenter;
using Core.GameEntity.Reaction;

namespace Core.Player.Presenter
{
    public class AnimationPlayerOnAttackStarted : AnimationPlayerOnReactionStarted<OnAttackStartedMessage>
    {
        protected override string GetAnimName() => "2H_Melee_Attack_Slice";

    }

}
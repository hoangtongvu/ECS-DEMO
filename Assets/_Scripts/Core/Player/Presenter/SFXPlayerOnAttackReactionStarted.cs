using Audio.JSAM;
using Core.GameEntity.Presenter;
using Core.GameEntity.Reaction;

namespace Core.Player.Presenter
{
    public class SFXPlayerOnAttackReactionStarted : SFXPlayerOnMessageInvoked<OnAttackStartedMessage, PlayerSound_LibrarySounds>
    {
    }
}
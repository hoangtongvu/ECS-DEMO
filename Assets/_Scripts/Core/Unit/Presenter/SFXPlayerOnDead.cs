using Audio.JSAM;
using Core.GameEntity.Presenter;
using Core.Misc.Presenter.PresenterMessages;

namespace Core.Unit.Presenter
{
    public class SFXPlayerOnDead : SFXPlayerOnMessageInvoked<OnDeadMessage, UnitSound_LibrarySounds>
    {
    }
}
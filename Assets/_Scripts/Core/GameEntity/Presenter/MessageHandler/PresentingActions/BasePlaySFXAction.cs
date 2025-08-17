using Core.Misc.Presenter;
using JSAM;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions
{
    [Serializable]
    public abstract class BasePlaySFXAction<SFXType> : PresentingAction
        where SFXType : Enum
    {
        [SerializeField] private SFXType sfx;

        public override void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }

        public override void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            AudioManager.PlaySound(this.sfx, baseGameObj.transform);
        }

        public override void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }
    }
}
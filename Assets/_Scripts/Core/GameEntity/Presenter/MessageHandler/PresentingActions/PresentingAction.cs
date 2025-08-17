using Core.Misc.Presenter;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions
{
    [Serializable]
    public abstract class PresentingAction
    {
        public abstract void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj);

        public abstract void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj);

        public abstract void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj);
    }
}
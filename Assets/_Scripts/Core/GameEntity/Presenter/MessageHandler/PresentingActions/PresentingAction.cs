using Core.Misc.Presenter;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions
{
    [Serializable]
    public abstract class PresentingAction
    {
        public abstract void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj);

        public abstract void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj);

        public abstract void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj);
    }

    [Serializable]
    public abstract class PresentingAction<TMessage>
        where TMessage : IMessage
    {
        public abstract void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj);

        public abstract void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj, [NotNull] TMessage message);

        public abstract void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj);
    }
}
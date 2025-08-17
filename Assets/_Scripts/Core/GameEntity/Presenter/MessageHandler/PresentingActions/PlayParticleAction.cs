using Core.Misc.Presenter;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions
{
    [Serializable]
    public class PlayParticleAction : PresentingAction
    {
        [SerializeField] private ParticleSystem target;

        public override void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }

        public override void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            this.target.Play();
        }

        public override void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }
    }
}
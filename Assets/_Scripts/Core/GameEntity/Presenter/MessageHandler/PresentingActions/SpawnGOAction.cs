using Core.Misc.Presenter;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions
{
    [Serializable]
    public class SpawnGOAction : PresentingAction
    {
        [SerializeField] private GameObject prefab;

        public override void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }

        public override void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            GameObject.Instantiate(this.prefab, baseGameObj.transform);
        }

        public override void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }
    }
}
using Core.Misc.Presenter;
using DSPool;
using System;
using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using UnityEngine;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions
{
    [Serializable]
    public class SpawnGOAction : PresentingAction
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private bool setBaseGOAsParent = true;
        [SerializeField] private GameObject spawnedGameObject;

        public override void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
        }

        public override void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            this.spawnedGameObject = SharedGameObjectPoolMap.Instance.Rent(this.prefab);

            if (this.setBaseGOAsParent)
            {
                this.spawnedGameObject.transform.SetParent(baseGameObj.transform);
                this.spawnedGameObject.transform.localPosition = float3.zero;
            }
            else
            {
                this.spawnedGameObject.transform.localPosition = baseGameObj.transform.position;
            }

            this.spawnedGameObject.SetActive(true);
        }

        public override void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            if (this.spawnedGameObject)
            {
                SharedGameObjectPoolMap.Instance.Return(this.spawnedGameObject);
            }
        }
    }
}
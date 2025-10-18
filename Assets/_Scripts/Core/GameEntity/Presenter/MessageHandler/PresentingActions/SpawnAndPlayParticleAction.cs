using Core.Misc.Presenter;
using DSPool;
using System.Diagnostics.CodeAnalysis;
using Unity.Mathematics;
using UnityEngine;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions
{
    [System.Serializable]
    public class SpawnAndPlayParticleAction : PresentingAction
    {
        [SerializeField] private ParticleSystem prefab;
        [SerializeField] private float3 localPosition;
        [SerializeField] private float3 localRotation;
        [SerializeField] private ParticleSystem target;

        public override void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            this.target = SharedGameObjectPoolMap.Instance.Rent(this.prefab.gameObject).GetComponent<ParticleSystem>();

            this.target.transform.SetParent(baseGameObj.transform);
            this.target.transform.SetLocalPositionAndRotation(this.localPosition, Quaternion.Euler(this.localRotation));
            this.target.gameObject.SetActive(true);
        }

        public override void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            this.target.Play();
        }

        public override void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            this.target.Stop();
        }
    }
}
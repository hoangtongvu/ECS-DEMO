using Core.Misc;
using UnityEngine;

namespace Core.ToolAndBuilding.ToolSpawnerBuilding.Presenter
{
    public class WorkBenchPresenter : SaiMonoBehaviour
    {
        [SerializeField] private ParticleSystem benchWorkingParticle;

        protected override void Awake()
        {
            this.benchWorkingParticle.Stop();
        }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.benchWorkingParticle);
            this.LoadBenchWorkingParticle();
        }

        private void LoadBenchWorkingParticle()
        {
            this.benchWorkingParticle = transform.Find("SmokeParticle").GetComponent<ParticleSystem>();
        }

        public void Work()
        {
            this.ShowVFX();
        }

        public void EndWorking()
        {
            this.HideVFX();
        }

        private void ShowVFX()
        {
            this.benchWorkingParticle.Play();
        }

        private void HideVFX()
        {
            this.benchWorkingParticle.Stop();
        }

    }

}
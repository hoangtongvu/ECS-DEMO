using Core.Misc.Presenter;
using System.Collections;
using UnityEngine;

namespace Core.Unit.Presenter
{
    public class UnitPresenter : BasePresenter
    {
        [SerializeField] private Renderer[] allChildRenderers;
        [SerializeField] private Material originalSharedMaterial;

        [SerializeField] private float flashDuration = 0.2f;
        private Coroutine currentFlashCoroutine;
        [field: SerializeField] public Material FlashMaterial { get; set; }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadAllChildRenderers();
            this.LoadOriginalSharedMaterial();
        }

        private void LoadAllChildRenderers()
        {
            this.allChildRenderers = GetComponentsInChildren<Renderer>();
        }

        private void LoadOriginalSharedMaterial()
        {
            // NOTE: We take the material from the first body part, assume that all body parts use the same material
            this.originalSharedMaterial = this.allChildRenderers[0].sharedMaterial;
        }

        public void ResetOriginalSharedMaterial()
        {
            int length = this.allChildRenderers.Length;

            for (int i = 0; i < length; i++)
            {
                this.allChildRenderers[i].sharedMaterial = this.originalSharedMaterial;
            }
        }

        public void SwapSharedMaterial(Material newMaterial)
        {
            int length = this.allChildRenderers.Length;

            for (int i = 0; i < length; i++)
            {
                this.allChildRenderers[i].sharedMaterial = newMaterial;
            }
        }

        public void OnTakeHit()
        {
            this.Flash();
        }

        public void Flash()
        {
            if (currentFlashCoroutine != null)
            {
                StopCoroutine(currentFlashCoroutine);
            }

            currentFlashCoroutine = StartCoroutine(nameof(DoFlash));
        }

        private IEnumerator DoFlash()
        {
            this.SwapSharedMaterial(this.FlashMaterial);
            yield return new WaitForSeconds(this.flashDuration);
            this.ResetOriginalSharedMaterial();
            this.currentFlashCoroutine = null;
        }

    }

}
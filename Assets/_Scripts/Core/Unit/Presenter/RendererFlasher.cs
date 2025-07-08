using Core.Misc;
using Core.Misc.Presenter;
using System.Collections;
using UnityEngine;

namespace Core.Unit.Presenter
{
    public class RendererFlasher : SaiMonoBehaviour
    {
        private Coroutine currentFlashCoroutine;
        [SerializeField] private float flashDuration = 0.2f;
        [SerializeField] private Material flashMaterial;
        [SerializeField] private Material originalSharedMaterial;
        [SerializeField] protected BasePresenter basePresenter;
        [SerializeField] private Renderer[] renderers;

        protected override void Awake()
        {
            this.LoadCtrl(ref this.basePresenter);
            this.LoadRenderers();
            this.LoadOriginalSharedMaterial();
        }

        private void LoadRenderers()
        {
            this.renderers = basePresenter.GetComponentsInChildren<Renderer>();
        }

        private void LoadOriginalSharedMaterial()
        {
            // NOTE: We take the material from the first body part, assume that all body parts use the same material
            this.originalSharedMaterial = this.renderers[0].sharedMaterial;
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
            this.SwapSharedMaterial(this.flashMaterial);
            yield return new WaitForSeconds(this.flashDuration);
            this.ResetOriginalSharedMaterial();
            this.currentFlashCoroutine = null;
        }

        public void ResetOriginalSharedMaterial()
        {
            int length = renderers.Length;

            for (int i = 0; i < length; i++)
            {
                renderers[i].sharedMaterial = this.originalSharedMaterial;
            }
        }

        public void SwapSharedMaterial(Material newMaterial)
        {
            int length = renderers.Length;

            for (int i = 0; i < length; i++)
            {
                renderers[i].sharedMaterial = newMaterial;
            }
        }
        
    }

}
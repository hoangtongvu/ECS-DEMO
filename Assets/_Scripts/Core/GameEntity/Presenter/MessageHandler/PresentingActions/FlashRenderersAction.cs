using Core.Misc.Presenter;
using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using UnityEngine;

namespace Core.GameEntity.Presenter.MessageHandler.PresentingActions
{
    [Serializable]
    public class FlashRenderersAction : PresentingAction
    {
        private CancellationTokenSource flashCts;

        [SerializeField] private float flashDurationSeconds = 0.2f;
        [SerializeField] private Material flashMaterial;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private Material[] originalMaterials;

        public override void Initialize([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            this.LoadRendersAndOriginalMaterials(basePresenter);
        }

        public override void Activate([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            this.Flash();
        }

        public override void Dispose([NotNull] BasePresenter basePresenter, [NotNull] GameObject baseGameObj)
        {
            this.flashCts?.Cancel();
            this.flashCts?.Dispose();
            this.flashCts = null;
        }

        private void LoadRendersAndOriginalMaterials(BasePresenter basePresenter)
        {
            this.renderers = basePresenter.GetComponentsInChildren<Renderer>();
            int length = this.renderers.Length;
            this.originalMaterials = new Material[length];

            for (int i = 0; i < length; i++)
            {
                this.originalMaterials[i] = this.renderers[i].sharedMaterial;
            }
        }

        public void Flash()
        {
            this.flashCts?.Cancel();
            this.flashCts?.Dispose();

            this.flashCts = new CancellationTokenSource();

            this.DoFlash(this.flashCts.Token).Forget();
        }

        private async UniTaskVoid DoFlash(CancellationToken token)
        {
            this.SwapSharedMaterials(this.flashMaterial);

            try
            {
                await UniTask.Delay(
                    (int)(this.flashDurationSeconds * 1000),
                    cancellationToken: token
                );
            }
            catch (OperationCanceledException)
            {
                return;
            }

            this.ResetOriginalSharedMaterials();
        }

        public void ResetOriginalSharedMaterials()
        {
            int length = this.renderers.Length;

            for (int i = 0; i < length; i++)
            {
                this.renderers[i].sharedMaterial = this.originalMaterials[i];
            }
        }

        public void SwapSharedMaterials(Material newMaterial)
        {
            int length = renderers.Length;

            for (int i = 0; i < length; i++)
            {
                renderers[i].sharedMaterial = newMaterial;
            }
        }

    }

}
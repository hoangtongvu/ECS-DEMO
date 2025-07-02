using Core.Animator;
using UnityEngine;

namespace Core.Misc.Presenter
{
    public class BasePresenter : SaiMonoBehaviour
    {
        [SerializeField] protected MeshRenderer meshRenderer;

        public MeshRenderer MeshRenderer => meshRenderer;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadMeshRenderer(out this.meshRenderer);
        }

        protected virtual void LoadMeshRenderer(out MeshRenderer meshRenderer) => meshRenderer = GetComponent<MeshRenderer>();

        public bool TryGetBaseAnimator(out BaseAnimator baseAnimator)
        {
            return this.gameObject.TryGetComponent<BaseAnimator>(out baseAnimator);
        }

    }

}
using Core.Animator;
using UnityEngine;

namespace Core.Misc.Presenter
{
    public class BasePresenter : SaiMonoBehaviour
    {
        [SerializeField] private BaseAnimator baseAnimator;

        public BaseAnimator BaseAnimator => baseAnimator;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(ref baseAnimator);
        }

    }
}
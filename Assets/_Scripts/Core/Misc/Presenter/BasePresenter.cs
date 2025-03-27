using Core.Animator;

namespace Core.Misc.Presenter
{
    public class BasePresenter : SaiMonoBehaviour
    {
        public bool TryGetBaseAnimator(out BaseAnimator baseAnimator)
        {
            return this.gameObject.TryGetComponent<BaseAnimator>(out baseAnimator);
        }

    }

}
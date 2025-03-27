using Core.Animator;
using Unity.Entities;

namespace Components.Misc.Presenter
{
    public struct AnimatorHolder : IComponentData
    {
        public UnityObjectRef<BaseAnimator> Value;
    }

}

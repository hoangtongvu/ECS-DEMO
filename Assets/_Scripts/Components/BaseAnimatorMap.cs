using Components.CustomIdentification;
using System.Collections.Generic;
using Unity.Entities;

namespace Components
{
    public class BaseAnimatorMap : IComponentData
    {
        public Dictionary<UniqueId, Core.Animator.BaseAnimator> Value;
    }
}
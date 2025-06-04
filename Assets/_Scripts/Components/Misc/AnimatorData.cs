using Core.Misc;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc
{
    public struct AnimatorData : IComponentData
    {
        public ChangedFlagValue<FixedString64Bytes> Value;
    }

}

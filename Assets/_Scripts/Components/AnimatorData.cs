using Unity.Collections;
using Unity.Entities;

namespace Components
{

    [InternalBufferCapacity(0)]
    public struct AnimationClipInfoElement : IBufferElementData
    {
        public FixedString64Bytes Name;
        public float Length;
    }

    public struct AnimatorData : IComponentData
    {
        public ChangedFlagValue<FixedString64Bytes> Value;
    }



    // Old version of AnimatorData using Property.

    //public struct AnimatorData : IComponentData
    //{
    //    [UnityEngine.SerializeField] private FixedString64Bytes animName;
    //    public FixedString64Bytes AnimName
    //    {
    //        get => animName;
    //        set
    //        {
    //            if (this.animName == value) return;
    //            animName = value;
    //            this.AnimChanged = true;
    //        }
    //    }
    //    public bool AnimChanged;
    //}

}

using Unity.Collections;
using Unity.Entities;

namespace Components
{

    public struct AnimatorData : IComponentData
    {
        public FixedString64Bytes AnimName;
        public bool AnimChanged;
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

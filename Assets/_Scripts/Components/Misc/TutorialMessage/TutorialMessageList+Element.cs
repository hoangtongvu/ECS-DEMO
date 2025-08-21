using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.TutorialMessage
{
    [System.Serializable]
    public struct TutorialMessageElement
    {
        public FixedString512Bytes String;
        public half TextDuration;
    }

    public struct TutorialMessageList : IComponentData
    {
        public NativeList<TutorialMessageElement> Value;
    }
}

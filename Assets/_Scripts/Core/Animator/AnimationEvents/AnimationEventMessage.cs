using ZBase.Foundation.PubSub;

namespace Core.Animator.AnimationEvents
{
    [System.Serializable]
    public struct AnimationEventMessage : IMessage
    {
        public int Id;
    }
}
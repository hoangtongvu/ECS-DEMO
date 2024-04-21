using Core.UI.Identification;
using ZBase.Foundation.PubSub;
using Unity.Mathematics;

namespace Core.MyEvent.PubSub.Messages
{
    public struct UISpawnMessage : IMessage
    {
        public UIType UIType;
        public float3 Position;
        public quaternion Quaternion;
        public UIID? ParentUIID;
    }

}
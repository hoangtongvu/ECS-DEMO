using ZBase.Foundation.PubSub;
using UnityEngine;
using Unity.Entities;

namespace Core.MyEvent.PubSub.Messages
{
    public struct RegisterMessage<TID, TTarget>: IMessage
        where TID : unmanaged
        where TTarget : Component
    {
        public TID ID;
        public UnityObjectRef<TTarget> TargetRef;
    }

    public struct RegisterMessage<TTarget>: IMessage
        where TTarget : Component
    {
        public UnityObjectRef<TTarget> TargetRef;
    }

}
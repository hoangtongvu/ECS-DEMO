using Components.Misc;
using Core.MyEvent.PubSub.Messages;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;

[assembly: RegisterGenericComponentType(typeof(MessageQueue<SpawnUnitMessage>))]

namespace Components.Misc
{
    public struct MessageQueue<TMessage> : IComponentData where TMessage : unmanaged, IMessage
    {
        public NativeQueue<TMessage> Value;
    }
}

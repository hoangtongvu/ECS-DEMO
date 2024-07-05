using Core.MyEvent.PubSub.Messages;
using Unity.Collections;
using Unity.Entities;

namespace Components
{

    public struct SpawnUnitMessageQueue : IComponentData
    {
        public NativeQueue<SpawnUnitMessage> Value;
    }

}

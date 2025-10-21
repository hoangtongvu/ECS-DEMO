using Components.Misc;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Collections;
using Unity.Entities;
using Utilities;
using ZBase.Foundation.PubSub;
using UnityEngine.Rendering;

namespace Systems.Initialization.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GlobalVolumeHolderRegisterSystem : SystemBase
    {
        private ISubscription subscription;
        private NativeQueue<RegisterMessage<Volume>> messageQueue;

        protected override void OnCreate()
        {
            this.messageQueue = new(Allocator.Persistent);

            this.subscription = MapRegisterMessenger.MessageSubscriber
                .Subscribe<RegisterMessage<Volume>>(message => this.messageQueue.Enqueue(message));
        }

        protected override void OnDestroy()
        {
            this.subscription.Dispose();
            this.messageQueue.Dispose();
        }

        protected override void OnUpdate()
        {
            while (this.messageQueue.TryDequeue(out var message))
            {
                var su = SingletonUtilities.GetInstance(this.EntityManager);

                su.AddOrSetComponentData(new GlobalVolumeHolder
                {
                    Value = message.TargetRef,
                });
            }
        }

    }

}
using Components.MyCamera;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Utilities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization.MyCamera
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class MainCamHolderInitSystem : SystemBase
    {
        private NativeQueue<RegisterMessage<Camera>> messageQueue;
        private ISubscription subscription;

        protected override void OnCreate()
        {
            this.messageQueue = new NativeQueue<RegisterMessage<UnityEngine.Camera>>(Allocator.Persistent);

            this.subscription = MapRegisterMessenger.MessageSubscriber
                .Subscribe<RegisterMessage<Camera>>(message => this.messageQueue.Enqueue(message));
        }

        protected override void OnUpdate()
        {
            while (messageQueue.TryDequeue(out var data))
            {
                this.Enabled = false;

                SingletonUtilities.GetInstance(this.EntityManager)
                    .AddOrSetComponentData(new MainCamHolder
                    {
                        Value = data.TargetRef,
                    });
            }

        }

        protected override void OnStopRunning()
        {
            this.subscription.Unsubscribe();
            this.messageQueue.Dispose();
        }

    }

}
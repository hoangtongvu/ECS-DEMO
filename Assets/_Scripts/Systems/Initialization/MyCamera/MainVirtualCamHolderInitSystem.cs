using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Collections;
using Unity.Entities;
using Utilities;
using ZBase.Foundation.PubSub;
using Cinemachine;
using Components.MyCamera;

namespace Systems.Initialization.MyCamera
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class MainVirtualCamHolderInitSystem : SystemBase
    {
        private NativeQueue<RegisterMessage<CinemachineVirtualCamera>> messageQueue;
        private ISubscription subscription;

        protected override void OnCreate()
        {
            this.messageQueue = new(Allocator.Persistent);

            this.subscription = MapRegisterMessenger.MessageSubscriber
                .Subscribe<RegisterMessage<CinemachineVirtualCamera>>(message => this.messageQueue.Enqueue(message));

        }

        protected override void OnUpdate()
        {
            while (this.messageQueue.TryDequeue(out var message))
            {
                SingletonUtilities.GetInstance(this.EntityManager)
                    .AddOrSetComponentData(new MainVirtualCamHolder
                    {
                        Value = message.TargetRef,
                    });

                this.Enabled = false;
                this.messageQueue.Dispose();
                this.subscription.Dispose();
                return;

            }

        }

    }

}
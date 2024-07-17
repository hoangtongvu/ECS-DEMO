using Components.Camera;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Collections;
using Unity.Entities;
using Utilities;
using ZBase.Foundation.PubSub;

namespace Systems.Simulation.Camera
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup), OrderFirst = true)]
    public partial class MainCamRegisterSystem : SystemBase
    {
        private NativeQueue<RegisterMessage<UnityEngine.Camera>> eventQueue;
        private ISubscription subscription;

        protected override void OnCreate()
        {
            this.eventQueue = new NativeQueue<RegisterMessage<UnityEngine.Camera>>(Allocator.Persistent);

            this.subscription = MapRegisterMessenger.MessageSubscriber
                .Subscribe<RegisterMessage<UnityEngine.Camera>>(this.HandleMessage);
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            while (this.eventQueue.TryDequeue(out var data))
            {
                this.HandleEvent(in data);
            }
        }

        protected override void OnDestroy()
        {
            this.subscription.Unsubscribe();
            this.eventQueue.Dispose();
        }


        private void HandleMessage(RegisterMessage<UnityEngine.Camera> data) => this.eventQueue.Enqueue(data);

        private void HandleEvent(in RegisterMessage<UnityEngine.Camera> data)
        {
            var mainCamHolder = new MainCamHolder
            {
                Value = data.TargetRef,
            };

            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(mainCamHolder);
        }

    }
}
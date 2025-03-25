using Components.UI.MyCanvas;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Core.UI.MyCanvas;
using Unity.Collections;
using Unity.Entities;
using Utilities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization.UI.MyCanvas
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class CanvasesCtrlHolderInitSystem : SystemBase
    {
        private NativeQueue<RegisterMessage<CanvasesCtrl>> messageQueue;
        private ISubscription subscription;

        protected override void OnCreate()
        {
            this.messageQueue = new(Allocator.Persistent);

            this.subscription = MapRegisterMessenger.MessageSubscriber
                .Subscribe<RegisterMessage<CanvasesCtrl>>(message => this.messageQueue.Enqueue(message));

        }

        protected override void OnUpdate()
        {
            while (this.messageQueue.TryDequeue(out var message))
            {
                SingletonUtilities.GetInstance(this.EntityManager)
                    .AddOrSetComponentData(new CanvasesCtrlHolder
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
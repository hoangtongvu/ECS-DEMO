using Components;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Collections;
using Unity.Entities;
using Utilities;
using ZBase.Foundation.PubSub;

namespace Systems.Simulation.Misc
{
    public partial class SpawnUnitMessageSystem : MessageQueuingSystem<SpawnUnitMessage> { }


    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class MessageQueuingSystem<TMessage> : SystemBase where TMessage : unmanaged, IMessage
    {
        private NativeQueue<TMessage> messages;

        protected override void OnCreate()
        {
            this.messages = new(Allocator.Persistent);

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new MessageQueue<TMessage>
                {
                    Value = new(Allocator.Persistent),
                });

            this.SubscribeEvents();
        }

        //protected override void OnDestroy()
        //{
        //    this.messages.Dispose(); // Note: This line cause Allocator Exception, dunno why.
        //    var messageQueue = SystemAPI.GetSingleton<MessageQueue<TMessage>>();
        //    messageQueue.Value.Dispose();
        //}

        protected override void OnUpdate()
        {
            var messageQueue = SystemAPI.GetSingleton<MessageQueue<TMessage>>();

            while (this.messages.TryDequeue(out var data))
            {
                messageQueue.Value.Enqueue(data);
            }
        }

        private void SubscribeEvents() => GameplayMessenger.MessageSubscriber.Subscribe<TMessage>(this.SpawnUnitMessageHandle);

        private void SpawnUnitMessageHandle(TMessage message) => this.messages.Enqueue(message);

    }

}

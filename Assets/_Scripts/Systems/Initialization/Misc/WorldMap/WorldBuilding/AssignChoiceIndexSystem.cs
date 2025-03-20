using Components.Misc.WorldMap.WorldBuilding;
using Core.MyEvent.PubSub.Messages.WorldBuilding;
using Core.MyEvent.PubSub.Messengers;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class AssignChoiceIndexSystem : SystemBase
    {
        private NativeQueue<ChooseBuildableObjectMessage> messageQueue;
        private ISubscription subscription;

        protected override void OnCreate()
        {
            this.messageQueue = new(Allocator.Persistent);

            this.subscription = GameplayMessenger.MessageSubscriber
                .Subscribe<ChooseBuildableObjectMessage>(message => this.messageQueue.Enqueue(message));

            this.RequireForUpdate<BuildableObjectChoiceIndex>();
        }

        protected override void OnUpdate()
        {
            var choiceIndexRef = SystemAPI.GetSingletonRW<BuildableObjectChoiceIndex>();

            while (this.messageQueue.TryDequeue(out var data))
            {
                choiceIndexRef.ValueRW.Value = data.choiceIndex;
            }

        }

        protected override void OnDestroy() => this.subscription.Dispose();

    }

}
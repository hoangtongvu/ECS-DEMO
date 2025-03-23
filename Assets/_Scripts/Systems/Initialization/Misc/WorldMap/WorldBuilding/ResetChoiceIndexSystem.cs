using Components.Misc.WorldMap.WorldBuilding;
using Core.MyEvent.PubSub.Messages.WorldBuilding;
using Core.MyEvent.PubSub.Messengers;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class ResetChoiceIndexSystem : SystemBase
    {
        private NativeQueue<BuildModeToggleMessage> messageQueue;
        private ISubscription subscription;

        protected override void OnCreate()
        {
            this.messageQueue = new(Allocator.Persistent);

            this.subscription = GameplayMessenger.MessageSubscriber
                .Subscribe<BuildModeToggleMessage>(message => this.messageQueue.Enqueue(message));

            this.RequireForUpdate<BuildableObjectChoiceIndex>();
        }

        protected override void OnUpdate()
        {
            var choiceIndexRef = SystemAPI.GetSingletonRW<BuildableObjectChoiceIndex>();

            while (this.messageQueue.TryDequeue(out var data))
            {
                if (choiceIndexRef.ValueRO.Value == BuildableObjectChoiceIndex.NoChoice) continue;
                choiceIndexRef.ValueRW.Value = BuildableObjectChoiceIndex.NoChoice;
            }

        }

        protected override void OnDestroy() => this.subscription.Dispose();

    }

}
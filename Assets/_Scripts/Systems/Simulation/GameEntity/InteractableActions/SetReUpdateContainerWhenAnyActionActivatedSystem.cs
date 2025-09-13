using Components.GameEntity.InteractableActions;
using Core.MyEvent.PubSub.Messengers;
using Core.UI.InteractableActionsPanel.ActionPanel;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SetReUpdateContainerWhenAnyActionActivatedSystem : SystemBase
    {
        private NativeQueue<OnAnyActionPanelActivatedMessage> messageQueue;
        private ISubscription subscription;

        protected override void OnCreate()
        {
            this.messageQueue = new(Allocator.Persistent);
            this.subscription = GameplayMessenger.MessageSubscriber
                .Subscribe<OnAnyActionPanelActivatedMessage>((message) => this.messageQueue.Enqueue(message));
        }

        protected override void OnDestroy()
        {
            this.messageQueue.Dispose();
            this.subscription.Dispose();
        }

        protected override void OnUpdate()
        {
            while (this.messageQueue.TryDequeue(out var _))
            {
                foreach (var canContainerUpdateTag in SystemAPI
                    .Query<EnabledRefRW<ActionsContainerUI_CD.CanUpdate>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
                {
                    canContainerUpdateTag.ValueRW = true;
                    return;
                }
            }
        }

    }

}
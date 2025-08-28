using Components.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel;
using Core.MyEvent.PubSub.Messages.WorldBuilding;
using Core.MyEvent.PubSub.Messengers;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class BuildableObjectsPanel_CanShowTagsHandleSystem : SystemBase
    {
        private ISubscription subscription;
        private NativeQueue<ToggleBuildModeMessage> messageQueue;

        protected override void OnCreate()
        {
            this.messageQueue = new(Allocator.Persistent);
            this.subscription = GameplayMessenger.MessageSubscriber
                .Subscribe<ToggleBuildModeMessage>(message => this.messageQueue.Enqueue(message));

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    BuildableObjectsPanel_CD.CanShow>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnDestroy()
        {
            this.subscription.Dispose();
        }

        protected override void OnUpdate()
        {
            while (this.messageQueue.TryDequeue(out var message))
            {
                foreach (var canShowTag in SystemAPI
                    .Query<
                        EnabledRefRW<BuildableObjectsPanel_CD.CanShow>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
                {
                    canShowTag.ValueRW = message.VisibleState;
                }
            }

        }

    }

}
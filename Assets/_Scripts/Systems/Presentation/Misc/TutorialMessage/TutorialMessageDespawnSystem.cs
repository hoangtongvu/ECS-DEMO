using Components.Misc.TutorialMessage;
using Components.UI.Pooling;
using Core.Misc.TutorialMessage;
using Core.MyEvent.PubSub.Messengers;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Presentation.Misc.TutorialMessage
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class TutorialMessageDespawnSystem : SystemBase
    {
        private ISubscription subscription;
        private NativeQueue<TutorialMessageDespawnedMessage> despawnMessageQueue;

        protected override void OnCreate()
        {
            this.despawnMessageQueue = new(Allocator.Persistent);
            this.subscription = GameplayMessenger.MessageSubscriber
                .Subscribe<TutorialMessageDespawnedMessage>((message) => this.despawnMessageQueue.Enqueue(message));

            this.RequireForUpdate<TutorialMessageSpawnedState>();
            this.RequireForUpdate<TutorialMessageCanDespawnState>();
            this.RequireForUpdate<SpawnedTutorialMessageCtrlHolder>();
            this.RequireForUpdate<UIPoolMapInitializedTag>();
        }

        protected override void OnDestroy()
        {
            this.subscription.Dispose();
        }

        protected override void OnUpdate()
        {
            var tutorialMessageSpawnedRef = SystemAPI.GetSingletonRW<TutorialMessageSpawnedState>();
            var spawnedMessageCtrlHolderRef = SystemAPI.GetSingletonRW<SpawnedTutorialMessageCtrlHolder>();

            // Despawn
            while (this.despawnMessageQueue.TryDequeue(out var _))
            {
                this.DespawnTutorialMessageCtrl(
                    ref tutorialMessageSpawnedRef.ValueRW
                    , ref spawnedMessageCtrlHolderRef.ValueRW);
            }

            // Trigger Despawn
            var tutorialMessageCanDespawnRef = SystemAPI.GetSingletonRW<TutorialMessageCanDespawnState>();
            if (!tutorialMessageCanDespawnRef.ValueRO.Value) return;

            if (!tutorialMessageSpawnedRef.ValueRO.Value) return;

            tutorialMessageCanDespawnRef.ValueRW.Value = false;
            spawnedMessageCtrlHolderRef.ValueRO.Value.Value.TutorialMessageTextEffectHandler.TriggerTextDisappear();

        }

        private void DespawnTutorialMessageCtrl(
            ref TutorialMessageSpawnedState tutorialMessageSpawnedState
            , ref SpawnedTutorialMessageCtrlHolder messageCtrlHolder)
        {
            messageCtrlHolder.Value.Value.ReturnSelfToPool();

            tutorialMessageSpawnedState.Value = false;
            messageCtrlHolder.Value = null;
        }

    }

}
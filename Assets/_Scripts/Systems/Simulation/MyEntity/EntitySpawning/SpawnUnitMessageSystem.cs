using Components;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Collections;
using Unity.Entities;
using Utilities;
using ZBase.Foundation.PubSub;

namespace Systems.Simulation.MyEntity.EntitySpawning
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SpawnUnitMessageSystem : SystemBase
    {
        private NativeQueue<SpawnUnitMessage> spawnUnitMessages;

        protected override void OnCreate()
        {
            this.spawnUnitMessages = new(Allocator.Persistent);

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new SpawnUnitMessageQueue
                {
                    Value = new(Allocator.Persistent),
                });

            this.SubscribeEvents();
        }


        protected override void OnUpdate()
        {
            var messageQueueRef = SystemAPI.GetSingletonRW<SpawnUnitMessageQueue>();

            while (this.spawnUnitMessages.TryDequeue(out var data))
            {
                messageQueueRef.ValueRW.Value.Enqueue(data);
                //UnityEngine.Debug.Log(data);
            }
        }

        private void SubscribeEvents()
        {
            GameplayMessenger.MessageSubscriber.Subscribe<SpawnUnitMessage>(this.SpawnUnitMessageHandle);
        }

        private void SpawnUnitMessageHandle(SpawnUnitMessage spawnUnitMessage)
        {
            this.spawnUnitMessages.Enqueue(spawnUnitMessage);
        }


    }


}

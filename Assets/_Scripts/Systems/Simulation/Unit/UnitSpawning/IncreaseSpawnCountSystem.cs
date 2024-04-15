using Unity.Entities;
using Components.Unit.UnitSpawning;
using Unity.Collections;
using Components.Unit;
using Components;
using ZBase.Foundation.PubSub;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Core.Extensions;
using Core.MyEvent.PubSub.Messengers;
using Core.MyEvent.PubSub.Messages;

namespace Systems.Simulation.Unit.UnitSpawning
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class IncreaseSpawnCountSystem : SystemBase
    {

        private NativeArray<ComponentsAndEventQueue> eventDataQueues;
        private NativeQueue<SpawnHouseUISystem.UIChangedMessage> uiChangedEventQueue;
        private List<ISubscription> subscriptions = new();


        protected override void OnCreate()
        {
            this.uiChangedEventQueue = new(Allocator.Persistent);
            GameplayMessenger.MessageSubscriber.Subscribe<SpawnHouseUISystem.UIChangedMessage>(this.UIChangedHandle);
        }

        protected override void OnUpdate()
        {

            /*
             * while uiChangedEventQueue.Dequeue(out var data)
             * {
             *      // Re-Update UI.
             *      Query UnitSelected HouseUICtrlRef UnitSpawningProfileElement
             *          if !selected return;
             *          
             *          int length = profileElements.Length; // this == number of buttons per UI.
             *          NativeQueue<ButtonMessage> queue = new();
             *          MessageSubscriber messageSubscriber = houseUICtrlRef.ValueRO.Value.Value.Messenger.MessageSubscriber;
             *          // clear previous Subscribers.
             *          
             *          EventHandler eh = new((spawningProfiles, queue));
             *          
             *          messageSubscriber.Sub(eh.Handle);
             *          
             *          tempList.Add((spawningProfiles, queue));
             *      
             * }
             * 
             * 
             * for i < 0; i++; i < number of UI // loop through NativeArr
             *      (spawningProfiles, queue) e = arr[i];
             *      int[] addAmounts = new int[this.eventDataQueues.Length];
             *      while e.queue.Dequeue(out var data)
             *          int messageID = data.id;
             *          addAmounts[messageID]++;
             *      
             *      for i in spawningProfiles
             *          ref p = spawningProfiles.ElementAt(i);
             *          p.spawnCount += addAmounts[i];
             *          
             * 
             */


            while (this.uiChangedEventQueue.TryDequeue(out var data))
            {
                // Re-Update UI.
                if (!this.eventDataQueues.IsCreated) this.DisposeEventQueues();
                this.UnSubAllEventMessages();

                NativeList<ComponentsAndEventQueue> tempList = new(Allocator.Temp);

                foreach (var (unitSelectedRef, houseUICtrlRef, profileElements) in
                    SystemAPI.Query<
                        RefRO<UnitSelected>
                        , RefRO<HouseUICtrlRef>
                        , DynamicBuffer<UnitSpawningProfileElement>>())
                {
                    if (!unitSelectedRef.ValueRO.Value) continue;
                    NativeQueue<ButtonMessage> queue = new(Allocator.Persistent);

                    var messSub = houseUICtrlRef.ValueRO.Value.Value.Messenger.MessageSubscriber;
                    // clear previous Subscribers.

                    EventHandler eh = new()
                    {
                        eventDataQueue = queue,
                    };

                    messSub.Subscribe<ButtonMessage>(eh.Handle).AddTo(this.subscriptions);

                    tempList.Add(new()
                    {
                        Queue = queue,
                        SpawningProfiles = profileElements,
                    });

                }

                this.eventDataQueues = tempList.ToArray(Allocator.Persistent);
                tempList.Dispose();

            }

            int length = this.eventDataQueues.Length;
            for (int i = 0; i < length; i++)
            {
                var componentsAndEventQueue = this.eventDataQueues[i];
                DynamicBuffer<UnitSpawningProfileElement> spawningProfiles = componentsAndEventQueue.SpawningProfiles;

                int[] addAmounts = new int[spawningProfiles.Length];

                while (componentsAndEventQueue.Queue.TryDequeue(out var data))
                {
                    int messageID = data.id;
                    addAmounts[messageID]++;
                }

                for (int j = 0; j < spawningProfiles.Length; j++)
                {
                    ref var profile = ref spawningProfiles.ElementAt(j);
                    profile.SpawnCount += addAmounts[j];
                }

            }


        }


        private struct EventHandler
        {
            public NativeQueue<ButtonMessage> eventDataQueue;

            public void Handle(ButtonMessage data) => this.eventDataQueue.Enqueue(data);
        }

        private struct ComponentsAndEventQueue
        {
            public DynamicBuffer<UnitSpawningProfileElement> SpawningProfiles;
            public NativeQueue<ButtonMessage> Queue;
        }

        private void UIChangedHandle(SpawnHouseUISystem.UIChangedMessage data) => this.uiChangedEventQueue.Enqueue(data);

        private void DisposeEventQueues()
        {
            int length = this.eventDataQueues.Length;
            for (int i = 0; i < length; i++)
            {
                this.eventDataQueues[i].Queue.Dispose();
            }
            this.eventDataQueues.Dispose();
        }

        private void UnSubAllEventMessages()
        {
            int count = subscriptions.Count;
            if (count <= 0) return;

            for (var i = 0; i < count; i++)
            {
                this.subscriptions[i]?.Unsubscribe();
            }

            this.subscriptions.Clear();
        }
    }
}
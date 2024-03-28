using Core.MyEvent;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SampleButtonEventListenerSystem : SystemBase
    {
        private NativeQueue<ButtonEventData> eventDataQueue;

        protected override void OnCreate()
        {
            this.eventDataQueue = new NativeQueue<ButtonEventData>(Allocator.Persistent);

            SampleButtonEventManager.testEvent.AddListener(this.HandleEvent);
        }

        protected override void OnUpdate()
        {
            while (this.eventDataQueue.TryDequeue(out var data))
            {
                UnityEngine.Debug.Log("Button clicked.");
            }


        }

        private void HandleEvent(ButtonEventData data)
        {
            this.eventDataQueue.Enqueue(data);
        }

    }
}

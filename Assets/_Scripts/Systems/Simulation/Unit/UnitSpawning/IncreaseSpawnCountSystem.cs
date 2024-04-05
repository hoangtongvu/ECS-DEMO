using Unity.Entities;
using Unity.Burst;
using Components.Unit.UnitSpawning;
using Unity.Collections;
using Core.MyEvent;

namespace Systems.Simulation.Unit.UnitSpawning
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct IncreaseSpawnCountSystem : ISystem
    {
        private NativeQueue<ButtonEventData> eventDataQueue;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.eventDataQueue = new (Allocator.Persistent);
            // Sub Event Here.
            SampleButtonEventManager.testEvent.AddListener(this.HandleEvent);

            state.RequireForUpdate<SpawnCount>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int addAmount = 0;
            while (this.eventDataQueue.TryDequeue(out var data))
            {
                addAmount++;
            }

            var job = new IncreaseSpawnCountJob
            {
                AddAmount = addAmount,
            };

            job.ScheduleParallel();
        }

        [BurstCompile]
        private void HandleEvent(ButtonEventData data) => this.eventDataQueue.Enqueue(data);

        [BurstCompile]
        private partial struct IncreaseSpawnCountJob : IJobEntity
        {
            public int AddAmount;
            void Execute(
                ref SpawnCount spawnCount)
            {
                spawnCount.Value += this.AddAmount;
            }
        }

    }
}
using Components.Unit.UnitSpawning;
using Components.Unit;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;

namespace Systems.Simulation.Unit.UnitSpawning
{

    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct IncSpawnCountSystem : ISystem
    {
        private NativeQueue<SpawnUnitMessage> spawnUnitMessages;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.spawnUnitMessages = new(Allocator.Persistent);
            GameplayMessenger.MessageSubscriber.Subscribe<SpawnUnitMessage>(this.SpawnUnitMessageHandle);
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Put foreach inside while loop is more efficient in this situation.
            while (this.spawnUnitMessages.TryDequeue(out var message))
            {
                foreach (var (unitSelectedRef, profiles) in
                    SystemAPI.Query<
                        RefRO<UnitSelected>
                        , DynamicBuffer<UnitSpawningProfileElement>>())
                {
                    if (!unitSelectedRef.ValueRO.Value) continue;

                    for (int i = 0; i < profiles.Length; i++)
                    {
                        ref var profile = ref profiles.ElementAt(i);

                        if (!profile.UIID.HasValue)
                        {
                            UnityEngine.Debug.LogError($"Profile UIID with order of {i} is null.");
                            continue;
                        }

                        if (!message.ProfileID.Equals(profile.UIID.Value)) continue;
                        profile.SpawnCount++;
                    }
                }
            }
        }

        private void SpawnUnitMessageHandle(SpawnUnitMessage spawnUnitMessage) => this.spawnUnitMessages.Enqueue(spawnUnitMessage);



    }


}

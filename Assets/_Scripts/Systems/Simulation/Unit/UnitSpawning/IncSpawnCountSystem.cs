using Components.Unit.UnitSpawning;
using Components.Unit;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;
using Core.UI.Identification;
using Components.GameResource;
using Core.Unit;
using Core.GameResource;

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
            this.SubscribeEvents();
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Only handle Player's side this time.
            var resourceWallet = SystemAPI.GetSingletonBuffer<ResourceWalletElement>();
            var unitCostMap = SystemAPI.GetSingleton<UnitCostMap>();

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

                        if (!this.IdMatched(message.ProfileID, profile.UIID.Value)) continue;
                        if (!this.HaveEnoughResources(resourceWallet, in unitCostMap, in profile, out var walletArr)) continue;

                        resourceWallet.CopyFrom(walletArr);
                        profile.SpawnCount++;
                    }
                }
            }
        }

        [BurstDiscard]
        private void SubscribeEvents()
        {
            GameplayMessenger.MessageSubscriber.Subscribe<SpawnUnitMessage>(this.SpawnUnitMessageHandle);
        }

        private void SpawnUnitMessageHandle(SpawnUnitMessage spawnUnitMessage) => this.spawnUnitMessages.Enqueue(spawnUnitMessage);

        [BurstCompile]
        private bool IdMatched(UIID first, UIID second) => first.Equals(second);

        // TODO: Turn this into job?
        [BurstCompile]
        private bool HaveEnoughResources(
            DynamicBuffer<ResourceWalletElement> resourceWallet
            , in UnitCostMap unitCostMap
            , in UnitSpawningProfileElement profile
            , out NativeArray<ResourceWalletElement> walletArr)
        {

            walletArr = resourceWallet.ToNativeArray(Allocator.Temp);

            int length = walletArr.Length;

            for (int i = 0; i < length; i++)
            {
                ResourceType resourceType = (ResourceType)i;

                var unitCostId = new UnitCostId(profile.UnitType, resourceType, profile.LocalIndex);

                if (!unitCostMap.Value.TryGetValue(unitCostId, out uint cost)) continue;
                long tempValue = (long) walletArr[i].Quantity - cost;
                // UnityEngine.Debug.Log($"{resourceType} {tempValue} = {walletArr[i].Quantity} - {cost}");

                if (tempValue < 0) return false;
                walletArr[i] = new ResourceWalletElement
                {
                    Type = resourceType,
                    Quantity = (uint)tempValue,
                };

            }

            return true;
        }


    }


}

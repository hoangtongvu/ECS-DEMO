using Components.MyEntity.EntitySpawning;
using Components.Unit;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using ZBase.Foundation.PubSub;
using Core.UI.Identification;
using Components.GameResource;
using Core.GameResource;
using System;

namespace Systems.Simulation.MyEntity.EntitySpawning
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
            var walletChangedRef = SystemAPI.GetSingletonRW<WalletChanged>();

            // Put foreach inside while loop is more efficient in this situation.
            while (this.spawnUnitMessages.TryDequeue(out var message))
            {
                foreach (var (unitSelectedRef, profiles, localCostMap) in
                    SystemAPI.Query<
                        RefRO<UnitSelected>
                        , DynamicBuffer<EntitySpawningProfileElement>
                        , DynamicBuffer<LocalCostMapElement>>())
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
                        if (!this.HaveEnoughResources(resourceWallet, localCostMap, i, out var walletArr)) continue;

                        resourceWallet.CopyFrom(walletArr);
                        walletChangedRef.ValueRW.Value = true;
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
            , DynamicBuffer<LocalCostMapElement> localCostMap
            , int bufferIndex
            , out NativeArray<ResourceWalletElement> walletArr)
        {

            walletArr = resourceWallet.ToNativeArray(Allocator.Temp);

            int length = walletArr.Length;

            for (int i = 0; i < length; i++)
            {
                ResourceType resourceType = (ResourceType)i;

                uint cost = this.GetCost(localCostMap, bufferIndex, resourceType);
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

        private uint GetCost(
            DynamicBuffer<LocalCostMapElement> localCostMap
            , int bufferIndex
            , ResourceType resourceType)
        {
            int enumLength = Enum.GetNames(typeof(ResourceType)).Length;
            int costMapIndex = bufferIndex * enumLength + (int) resourceType;
            return localCostMap[costMapIndex].Cost;
        }


    }


}

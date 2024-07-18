using Components.MyEntity.EntitySpawning;
using Components.Unit;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Core.UI.Identification;
using Components.GameResource;
using Core.GameResource;
using Components;
using Core.MyEvent.PubSub.Messages;

namespace Systems.Simulation.MyEntity.EntitySpawning
{

    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct IncSpawnCountSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var entityQuery0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ResourceWalletElement
                    , WalletChanged>()
                    .Build();

            var entityQuery1 = SystemAPI.QueryBuilder()
                .WithAll<
                    MessageQueue<SpawnUnitMessage>
                    , EnumLength<ResourceType>>()
                    .Build();

            var entityQuery2 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitSelected
                    , EntitySpawningProfileElement
                    , LocalCostMapElement>()
                    .Build();


            state.RequireForUpdate(entityQuery0);
            state.RequireForUpdate(entityQuery1);
            state.RequireForUpdate(entityQuery2);

        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Only handle Player's side this time.
            var resourceWallet = SystemAPI.GetSingletonBuffer<ResourceWalletElement>();
            var walletChangedRef = SystemAPI.GetSingletonRW<WalletChanged>();
            var messageQueue = SystemAPI.GetSingleton<MessageQueue<SpawnUnitMessage>>();
            var resourceCount = SystemAPI.GetSingleton<EnumLength<ResourceType>>();

            // Put foreach inside while loop is more efficient in this situation.
            while (messageQueue.Value.TryDequeue(out var message))
            {
                // Turn whole these below into IJobEntity?
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

                        if (!profile.CanIncSpawnCount) continue;

                        if (!profile.UIID.HasValue)
                        {
                            UnityEngine.Debug.LogError($"Profile UIID with order of {i} is null.");
                            continue;
                        }

                        if (!this.IdMatched(message.ProfileID, profile.UIID.Value)) continue;
                        if (!this.HaveEnoughResources(
                            resourceWallet
                            , localCostMap
                            , i
                            , resourceCount.Value
                            , out var walletArr)) continue;

                        resourceWallet.CopyFrom(walletArr);
                        walletChangedRef.ValueRW.Value = true;

                        profile.SpawnCount.ChangeValue(profile.SpawnCount.Value + 1);
                    }
                }
            }
        }


        [BurstCompile]
        private bool IdMatched(UIID first, UIID second) => first.Equals(second);

        // TODO: Turn this into job?
        [BurstCompile]
        private bool HaveEnoughResources(
            DynamicBuffer<ResourceWalletElement> resourceWallet
            , DynamicBuffer<LocalCostMapElement> localCostMap
            , int bufferIndex
            , int resourceCount
            , out NativeArray<ResourceWalletElement> walletArr)
        {

            walletArr = resourceWallet.ToNativeArray(Allocator.Temp);

            int length = walletArr.Length;

            for (int i = 0; i < length; i++)
            {
                ResourceType resourceType = (ResourceType)i;

                uint cost = this.GetCost(
                    localCostMap
                    , bufferIndex
                    , resourceCount
                    , resourceType);

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

        [BurstCompile]
        private uint GetCost(
            DynamicBuffer<LocalCostMapElement> localCostMap
            , int bufferIndex
            , int resourceCount
            , ResourceType resourceType)
        {
            int costMapIndex = bufferIndex * resourceCount + (int) resourceType;
            return localCostMap[costMapIndex].Cost;
        }


    }


}

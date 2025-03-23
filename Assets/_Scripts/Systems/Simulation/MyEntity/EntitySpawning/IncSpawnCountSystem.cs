using Components.MyEntity.EntitySpawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Core.UI.Identification;
using Components.GameResource;
using Core.GameResource;
using Components;
using Core.MyEvent.PubSub.Messages;
using Utilities.Extensions;
using Components.Unit.UnitSelection;
using Components.Player;

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
                    , WalletChangedTag>()
                    .Build();

            var entityQuery1 = SystemAPI.QueryBuilder()
                .WithAll<
                    MessageQueue<SpawnUnitMessage>
                    , EnumLength<ResourceType>>()
                    .Build();

            var entityQuery2 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitSelectedTag
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
            var messageQueue = SystemAPI.GetSingleton<MessageQueue<SpawnUnitMessage>>();
            var resourceCount = SystemAPI.GetSingleton<EnumLength<ResourceType>>();

            DynamicBuffer<ResourceWalletElement> resourceWallet = default;
            EnabledRefRW<WalletChangedTag> walletChangedTag = default;

            foreach (var item in
                SystemAPI.Query<
                    DynamicBuffer<ResourceWalletElement>
                    , EnabledRefRW<WalletChangedTag>>()
                    .WithAll<PlayerTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                resourceWallet = item.Item1;
                walletChangedTag = item.Item2;
            }

            while (messageQueue.Value.TryDequeue(out var message))
            {
                var profiles = SystemAPI.GetBuffer<EntitySpawningProfileElement>(message.SpawnerEntity);
                var localCostMap = SystemAPI.GetBuffer<LocalCostMapElement>(message.SpawnerEntity);

                ref var profile = ref profiles.ElementAt(message.SpawningProfileElementIndex);

                if (!this.HaveEnoughResources(
                    resourceWallet
                    , localCostMap
                    , message.SpawningProfileElementIndex
                    , resourceCount.Value
                    , out var walletArr)) continue;

                resourceWallet.CopyFrom(walletArr);
                walletChangedTag.ValueRW = true;

                profile.SpawnCount.ChangeValue(profile.SpawnCount.Value + 1);

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

using Components.GameEntity.Damage;
using Components.GameEntity.Misc;
using Components.GameResource;
using Components.Player;
using Components.Unit.Misc;
using Components.Unit.UnitFeeding;
using Core.GameResource;
using Core.Unit.UnitFeeding;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Utilities;
using Utilities.Helpers;

namespace Systems.Simulation.Unit.UnitFeeding
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct FeedingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddComponent<FeedingTimerSeconds>();

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    IsAlive>()
                .WithAll<
                    UnitTag
                    , FactionIndex
                    , HungerBarValue
                    , CurrentHungerThreshold>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<FeedingTimerSeconds>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var configs = UnitFeedingConfigConstants.UnitFeedingConfigs;
            var timerSecondsRef = SystemAPI.GetSingletonRW<FeedingTimerSeconds>();

            float feedingIntervalSeconds = configs.FeedingEventConfigs.FeedingIntervalMinutes * 60;
            timerSecondsRef.ValueRW += SystemAPI.Time.DeltaTime;

            if (timerSecondsRef.ValueRO < feedingIntervalSeconds) return;

            timerSecondsRef.ValueRW = 0;
            this.GetPlayerComponents(ref state, out var resourceWallet, out var walletChangedTag, out var playerFactionIndex);

            float barCap = configs.HungerBarConfigs.HungerBarCap;
            uint foodPerUnitPerFeedingEvent = configs.FeedingEventConfigs.FoodPerUnitPerFeedingEvent;
            float hungerValuePerFood = configs.HungerBarConfigs.HungerValuePerFood;
            float barGainPerFeedingEvent = foodPerUnitPerFeedingEvent * hungerValuePerFood;

            foreach (var (factionIndexRef, currentHungerThresholdRef, hungerBarValueRef) in SystemAPI
                .Query<
                    RefRO<FactionIndex>
                    , RefRO<CurrentHungerThreshold>
                    , RefRW<HungerBarValue>>()
                .WithAll<UnitTag>()
                .WithAll<IsAlive>())
            {
                if (factionIndexRef.ValueRO.Value != playerFactionIndex.Value) continue;
                if (currentHungerThresholdRef.ValueRO == HungerThreshold.Full) continue;

                bool hasEnoughFood = ResourceWalletHelper.TrySpendResourceOfType(
                    ref resourceWallet
                    , ref walletChangedTag
                    , ResourceType.Food
                    , foodPerUnitPerFeedingEvent);

                if (!hasEnoughFood) break;

                hungerBarValueRef.ValueRW = math.min(barCap, hungerBarValueRef.ValueRO + barGainPerFeedingEvent);
            }

        }

        [BurstCompile]
        private void GetPlayerComponents(
            ref SystemState state
            , out DynamicBuffer<ResourceWalletElement> resourceWallet
            , out EnabledRefRW<WalletChangedTag> walletChangedTag
            , out FactionIndex factionIndex)
        {
            resourceWallet = default;
            walletChangedTag = default;
            factionIndex = default;

            foreach (var item in
                SystemAPI.Query<
                    DynamicBuffer<ResourceWalletElement>
                    , EnabledRefRW<WalletChangedTag>
                    , RefRO<FactionIndex>>()
                    .WithAll<PlayerTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                resourceWallet = item.Item1;
                walletChangedTag = item.Item2;
                factionIndex = item.Item3.ValueRO;
            }
        }

    }

}
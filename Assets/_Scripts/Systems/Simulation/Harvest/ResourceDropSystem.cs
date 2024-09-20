using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Core.Harvest;
using Components.GameResource;
using Core.GameResource;

namespace Systems.Simulation.Harvest
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(HarvestSystem))]
    [BurstCompile]
    public partial struct ResourceDropSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    HarvesteeProfileIdHolder
                    , DropResourceHpThreshold
                    , HarvesteeHealthChangedTag>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var harvesteeHealthMap = SystemAPI.GetSingleton<HarvesteeHealthMap>();
            

            foreach (var (profileIdRef, dropResourceHpThresholdRef, harvesteeEntity) in
                SystemAPI.Query<
                    RefRO<HarvesteeProfileIdHolder>
                    , RefRW<DropResourceHpThreshold>>()
                    .WithAll<HarvesteeHealthChangedTag>()
                    .WithEntityAccess())
            {

                uint currentHp = this.GetCurrentHp(in harvesteeHealthMap, in harvesteeEntity);
                var harvesteeProfile = this.GetHarvesteeProfile(in profileIdRef.ValueRO.Value);


                uint hpThreshold = dropResourceHpThresholdRef.ValueRO.Value;
                uint deductAmount = harvesteeProfile.ResourceDropInfo.HpAmountPerDrop;
                uint quantityPerDrop = harvesteeProfile.ResourceDropInfo.QuantityPerDrop;

                while (currentHp <= hpThreshold)
                {
                    this.DropResources(
                        harvesteeProfile.ResourceDropInfo.ResourceType
                        , quantityPerDrop);


                    if (hpThreshold < deductAmount)
                    {
                        hpThreshold = 0;
                        break;
                    }

                    hpThreshold -= deductAmount;

                }

                dropResourceHpThresholdRef.ValueRW.Value = hpThreshold;


            }

        }

        [BurstCompile]
        private uint GetCurrentHp(in HarvesteeHealthMap harvesteeHealthMap, in Entity harvesteeEntity)
        {
            var healthId = new HealthId
            {
                Index = harvesteeEntity.Index,
                Version = harvesteeEntity.Version,
            };

            if (!harvesteeHealthMap.Value.TryGetValue(healthId, out var currentHp))
            {
                UnityEngine.Debug.LogError($"HarvesteeHealthMap does not contain {healthId}");
                return 0;
            }

            return currentHp;
        }

        [BurstCompile]
        private HarvesteeProfile GetHarvesteeProfile(in HarvesteeProfileId harvesteeProfileId)
        {
            var harvesteeProfileMap = SystemAPI.GetSingleton<HarvesteeProfileMap>();

            if (!harvesteeProfileMap.Value.TryGetValue(harvesteeProfileId, out var harvesteeProfile))
            {
                UnityEngine.Debug.LogError($"HarvesteeProfileMap does not contain {harvesteeProfileId}");
                return default;
            }

            return harvesteeProfile;
        }

        [BurstCompile]
        private void DropResources(
            ResourceType dropType
            , uint quantityPerDrop)
        {
            var resourceWallet = SystemAPI.GetSingletonBuffer<ResourceWalletElement>();
            var walletChangedRef = SystemAPI.GetSingletonRW<WalletChanged>(); //TODO: fix this Bad code
            int walletLength = resourceWallet.Length;

            for (int i = 0; i < walletLength; i++)
            {
                ref var walletElement = ref resourceWallet.ElementAt(i);

                bool matchType = walletElement.Type == dropType;
                if (!matchType) continue;

                walletElement.Quantity += quantityPerDrop;
                walletChangedRef.ValueRW.Value = true;
                break;

            }
        }

    }
}
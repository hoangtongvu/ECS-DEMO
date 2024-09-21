using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Core.Harvest;
using Components.GameResource;
using Core.GameResource;
using Unity.Mathematics;
using Unity.Transforms;
using Core.Utilities.Extensions;

namespace Systems.Simulation.Harvest
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(HarvestSystem))]
    [BurstCompile]
    public partial struct ResourceDropSystem : ISystem
    {

        private Random rand;

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

            this.rand = new Random(1);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var harvesteeHealthMap = SystemAPI.GetSingleton<HarvesteeHealthMap>();
            var harvesteeProfileMap = SystemAPI.GetSingleton<HarvesteeProfileMap>();
            var itemSpawnCommandList = SystemAPI.GetSingleton<ResourceItemSpawnCommandList>();

            foreach (var (profileIdRef, dropResourceHpThresholdRef, transformRef, harvesteeEntity) in
                SystemAPI.Query<
                    RefRO<HarvesteeProfileIdHolder>
                    , RefRW<DropResourceHpThreshold>
                    , RefRO<LocalTransform>>()
                    .WithAll<HarvesteeHealthChangedTag>()
                    .WithEntityAccess())
            {

                uint currentHp = this.GetCurrentHp(in harvesteeHealthMap, in harvesteeEntity);
                var harvesteeProfile = this.GetHarvesteeProfile(in harvesteeProfileMap, in profileIdRef.ValueRO.Value);


                uint hpThreshold = dropResourceHpThresholdRef.ValueRO.Value;
                uint deductAmount = harvesteeProfile.ResourceDropInfo.HpAmountPerDrop;
                uint quantityPerDrop = harvesteeProfile.ResourceDropInfo.QuantityPerDrop;

                while (currentHp <= hpThreshold)
                {
                    this.DropResources(
                        in itemSpawnCommandList
                        , transformRef.ValueRO.Position
                        , harvesteeProfile.ResourceDropInfo.ResourceType
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
        private HarvesteeProfile GetHarvesteeProfile(
            in HarvesteeProfileMap harvesteeProfileMap
            , in HarvesteeProfileId harvesteeProfileId)
        {

            if (!harvesteeProfileMap.Value.TryGetValue(harvesteeProfileId, out var harvesteeProfile))
            {
                UnityEngine.Debug.LogError($"HarvesteeProfileMap does not contain {harvesteeProfileId}");
                return default;
            }

            return harvesteeProfile;
        }

        [BurstCompile]
        private void DropResources(
            in ResourceItemSpawnCommandList spawnCommandList
            , float3 centerPos
            , ResourceType dropType
            , uint quantityPerDrop)
        {
            float2 randomDir = this.rand.NextFloat2Direction();
            const float dropRadius = 1.5f;
            float2 distanceFloat2 = randomDir * dropRadius;

            float3 spawnPos = centerPos.Add(x: distanceFloat2.x, z: distanceFloat2.y);

            spawnCommandList.Value.Add(new ResourceItemSpawnCommand
            {
                SpawnPos = spawnPos,
                ResourceType = dropType,
                Quantity = quantityPerDrop,
            });

        }

    }
}
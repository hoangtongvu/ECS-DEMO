using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Components.GameResource;
using Core.GameResource;
using Unity.Mathematics;
using Unity.Transforms;
using Core.Utilities.Extensions;
using Components.GameEntity;
using Components.Harvest.HarvesteeHp;
using System.Collections.Generic;

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
                    DropResourceHpThreshold
                    , HarvesteeHpChangedTag>()
                .Build();

            state.RequireForUpdate(query0);

            this.rand = new Random(1);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var currentHpMap = SystemAPI.GetSingleton<HarvesteeCurrentHpMap>();
            var itemSpawnCommandList = SystemAPI.GetSingleton<ResourceItemSpawnCommandList>();
            var resourceDropInfoMap = SystemAPI.GetSingleton<HarvesteeResourcceDropInfoMap>().Value;

            foreach (var (dropResourceHpThresholdRef, transformRef, primaryPrefabEntityHolderRef, harvesteeEntity) in
                SystemAPI.Query<
                    RefRW<DropResourceHpThreshold>
                    , RefRO<LocalTransform>
                    , RefRO<PrimaryPrefabEntityHolder>>()
                    .WithAll<HarvesteeHpChangedTag>()
                    .WithEntityAccess())
            {
                var resourceDropInfo = resourceDropInfoMap[primaryPrefabEntityHolderRef.ValueRO];

                uint currentHp = this.GetCurrentHp(in currentHpMap, in harvesteeEntity);

                uint hpThreshold = dropResourceHpThresholdRef.ValueRO.Value;
                uint deductAmount = resourceDropInfo.HpAmountPerDrop;
                uint quantityPerDrop = resourceDropInfo.QuantityPerDrop;

                while (currentHp <= hpThreshold)
                {
                    this.DropResources(
                        in itemSpawnCommandList
                        , transformRef.ValueRO.Position
                        , resourceDropInfo.ResourceType
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
        private uint GetCurrentHp(in HarvesteeCurrentHpMap currentHpMap, in Entity harvesteeEntity)
        {
            if (!currentHpMap.Value.TryGetValue(harvesteeEntity, out var currentHp))
                throw new KeyNotFoundException($"{nameof(HarvesteeCurrentHpMap)} does not contain key: {harvesteeEntity}");

            return currentHp;
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
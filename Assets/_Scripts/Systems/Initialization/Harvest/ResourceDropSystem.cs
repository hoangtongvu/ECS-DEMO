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
using Systems.Initialization.GameEntity.Damage;
using Components.GameEntity.Damage;

namespace Systems.Initialization.Harvest
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(HpChangeHandleSystem))]
    [BurstCompile]
    public partial struct ResourceDropSystem : ISystem
    {
        private Random rand;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CurrentHp
                    , DropResourceHpThreshold
                    , LocalTransform
                    , PrimaryPrefabEntityHolder>()
                .WithAll<IsAliveTag>()
                .Build();

            state.RequireForUpdate(query0);

            this.rand = new Random(1);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var itemSpawnCommandList = SystemAPI.GetSingleton<ResourceItemSpawnCommandList>();
            var resourceDropInfoMap = SystemAPI.GetSingleton<HarvesteeResourceDropInfoMap>().Value;

            // TODO: Fix missing last drop due to the IsAliveTag
            foreach (var (currentHpRef, dropResourceHpThresholdRef, transformRef, primaryPrefabEntityHolderRef) in
                SystemAPI.Query<
                    RefRO<CurrentHp>
                    , RefRW<DropResourceHpThreshold>
                    , RefRO<LocalTransform>
                    , RefRO<PrimaryPrefabEntityHolder>>()
                    .WithAll<IsAliveTag>())
            {
                var resourceDropInfo = resourceDropInfoMap[primaryPrefabEntityHolderRef.ValueRO];

                uint currentHp = (uint)currentHpRef.ValueRO.Value;

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
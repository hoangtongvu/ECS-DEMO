using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Components.GameResource;
using Core.GameResource;
using Unity.Mathematics;
using Unity.Transforms;
using Components.GameEntity;
using Components.Harvest.HarvesteeHp;
using Systems.Initialization.GameEntity.Damage;
using Components.GameEntity.Damage;

namespace Systems.Initialization.Harvest
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup))]
    [BurstCompile]
    public partial struct ResourceDropSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CurrentHp
                    , DropResourceHpThreshold
                    , LocalTransform
                    , PrimaryPrefabEntityHolder>()
                .WithAll<
                    IsAliveTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var itemSpawnCommandList = SystemAPI.GetSingleton<ResourceItemSpawnCommandList>();
            var resourceDropInfoMap = SystemAPI.GetSingleton<HarvesteeResourceDropInfoMap>().Value;

            foreach (var (currentHpRef, dropResourceHpThresholdRef, transformRef, primaryPrefabEntityHolderRef) in SystemAPI
                .Query<
                    RefRO<CurrentHp>
                    , RefRW<DropResourceHpThreshold>
                    , RefRO<LocalTransform>
                    , RefRO<PrimaryPrefabEntityHolder>>()
                .WithAll<
                    IsAliveTag>())
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

                    if (hpThreshold == 0)
                        break;

                    hpThreshold = math.max(0, hpThreshold - deductAmount);
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
            spawnCommandList.Value.Add(new ResourceItemSpawnCommand
            {
                SpawnPos = centerPos,
                ResourceType = dropType,
                Quantity = quantityPerDrop,
            });

        }

    }

}
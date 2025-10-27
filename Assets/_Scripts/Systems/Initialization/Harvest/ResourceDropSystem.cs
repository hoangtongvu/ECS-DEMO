using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Components.GameResource;
using Core.GameResource;
using Unity.Mathematics;
using Unity.Transforms;
using Components.GameEntity;
using Components.Harvest.HarvesteeHp;
using Components.GameEntity.Damage;
using Systems.Initialization.GameEntity.Damage.HpChangesHandle;

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
                    TakeHitEvent>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var itemSpawnCommandList = SystemAPI.GetSingleton<ResourceItemSpawnCommandList>();
            var resourceDropInfoMap = SystemAPI.GetSingleton<HarvesteeResourceDropInfoMap>().Value;

            foreach (var (currentHpRef, dropResourceHpThresholdRef, transformRef, primaryPrefabEntityHolderRef, entity) in SystemAPI
                .Query<
                    RefRO<CurrentHp>
                    , RefRW<DropResourceHpThreshold>
                    , RefRO<LocalTransform>
                    , RefRO<PrimaryPrefabEntityHolder>>()
                .WithAll<TakeHitEvent>()
                .WithEntityAccess())
            {
                var resourceDropInfo = resourceDropInfoMap[primaryPrefabEntityHolderRef.ValueRO];

                uint currentHp = (uint)currentHpRef.ValueRO.valueOfInt32;

                uint hpThreshold = dropResourceHpThresholdRef.ValueRO.Value;
                uint deductAmount = resourceDropInfo.HpAmountPerDrop;
                uint quantityPerDrop = resourceDropInfo.QuantityPerDrop;

                while (currentHp <= hpThreshold)
                {
                    this.DropResources(
                        in itemSpawnCommandList
                        , transformRef.ValueRO.Position
                        , resourceDropInfo.ResourceType
                        , quantityPerDrop
                        , in entity);

                    if (hpThreshold == 0)
                        break;

                    hpThreshold = (uint)math.max(0, (int)hpThreshold - deductAmount);
                }

                dropResourceHpThresholdRef.ValueRW.Value = hpThreshold;
            }

        }

        [BurstCompile]
        private void DropResources(
            in ResourceItemSpawnCommandList spawnCommandList
            , float3 centerPos
            , ResourceType dropType
            , uint quantityPerDrop
            , in Entity dropper)
        {
            spawnCommandList.Value.Add(new ResourceItemSpawnCommand
            {
                SpawnerEntity = dropper,
                SpawnPos = centerPos,
                ResourceType = dropType,
                Quantity = quantityPerDrop,
            });
        }

    }

}
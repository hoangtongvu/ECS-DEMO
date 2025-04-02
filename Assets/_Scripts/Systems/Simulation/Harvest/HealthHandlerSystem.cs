using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Unity.Collections;
using Utilities;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity;
using Components.Harvest.HarvesteeHp;

namespace Systems.Simulation.Harvest
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct HealthHandlerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new HarvesteeCurrentHpMap
                {
                    Value = new(200, Allocator.Persistent),
                });

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    DropResourceHpThreshold
                    , NewlySpawnedTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<HarvesteeMaxHpMap>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var maxHpMap = SystemAPI.GetSingleton<HarvesteeMaxHpMap>().Value;
            var currentHpMap = SystemAPI.GetSingleton<HarvesteeCurrentHpMap>();
            var resourceDropInfoMap = SystemAPI.GetSingleton<HarvesteeResourcceDropInfoMap>().Value;

            foreach (var (dropThresholdRef, primaryPrefabEntityHolderRef, harvesteeEntity) in
                SystemAPI.Query<
                    RefRW<DropResourceHpThreshold>
                    , RefRO<PrimaryPrefabEntityHolder>>()
                    .WithAll<NewlySpawnedTag>()
                    .WithEntityAccess())
            {
                uint maxHp = maxHpMap[primaryPrefabEntityHolderRef.ValueRO];
                var resourceDropInfo = resourceDropInfoMap[primaryPrefabEntityHolderRef.ValueRO];

                uint hpAmountPerDrop = resourceDropInfo.HpAmountPerDrop;

                this.InitCurrentHp(
                    in currentHpMap
                    , in harvesteeEntity
                    , maxHp);

                this.InitHpThreshold(
                    ref dropThresholdRef.ValueRW
                    , maxHp
                    , hpAmountPerDrop);

            }

        }

        [BurstCompile]
        private void InitCurrentHp(
            in HarvesteeCurrentHpMap currentHpMap
            , in Entity harvesteeEntity
            , uint maxHp)
        {
            currentHpMap.Value.Add(harvesteeEntity, maxHp);

        }

        [BurstCompile]
        private void InitHpThreshold(
            ref DropResourceHpThreshold hpThreshold
            , uint maxHp
            , uint hpAmountPerDrop)
        {
            hpThreshold.Value = maxHp - hpAmountPerDrop;
        }

    }

}
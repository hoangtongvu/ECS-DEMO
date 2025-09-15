using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity;
using Components.Harvest.HarvesteeHp;
using Components.GameEntity.Damage;

namespace Systems.Initialization.Harvest.HarvesteeHp
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct HpThresholdInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    DropResourceHpThreshold
                    , HpDataHolder
                    , PrimaryPrefabEntityHolder
                    , NewlySpawnedTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<HarvesteeResourceDropInfoMap>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var resourceDropInfoMap = SystemAPI.GetSingleton<HarvesteeResourceDropInfoMap>().Value;

            foreach (var (dropThresholdRef, hpDataHolder, primaryPrefabEntityHolderRef) in
                SystemAPI.Query<
                    RefRW<DropResourceHpThreshold>
                    , HpDataHolder
                    , RefRO<PrimaryPrefabEntityHolder>>()
                    .WithAll<NewlySpawnedTag>())
            {
                var resourceDropInfo = resourceDropInfoMap[primaryPrefabEntityHolderRef.ValueRO];
                uint hpAmountPerDrop = resourceDropInfo.HpAmountPerDrop;

                this.InitHpThreshold(
                    ref dropThresholdRef.ValueRW
                    , (uint)hpDataHolder.value.MaxHp
                    , hpAmountPerDrop);

            }

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
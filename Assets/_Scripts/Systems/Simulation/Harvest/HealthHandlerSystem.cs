using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Unity.Collections;
using Utilities;
using Core.Harvest;
using Components.GameEntity.EntitySpawning;

namespace Systems.Simulation.Harvest
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct HealthHandlerSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.CreateMap(ref state);

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    HarvesteeProfileIdHolder
                    , HarvesteeHealthId
                    , DropResourceHpThreshold
                    , NewlySpawnedTag>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var (profileIdRef, healthIdRef, dropThresholdRef, harvesteeEntity) in
                SystemAPI.Query<
                    RefRO<HarvesteeProfileIdHolder>
                    , RefRW<HarvesteeHealthId>
                    , RefRW<DropResourceHpThreshold>>()
                    .WithAll<NewlySpawnedTag>()
                    .WithEntityAccess())
            {

                var harvesteeProfile = this.GetHarvesteeProfile(in profileIdRef.ValueRO.Value);

                uint maxHp = harvesteeProfile.MaxHp;
                uint hpAmountPerDrop = harvesteeProfile.ResourceDropInfo.HpAmountPerDrop;

                this.InitCurrentHp(
                    in harvesteeEntity
                    , ref healthIdRef.ValueRW
                    , maxHp);

                this.InitHpThreshold(
                    ref dropThresholdRef.ValueRW
                    , maxHp
                    , hpAmountPerDrop);

            }

        }

        [BurstCompile]
        private void CreateMap(ref SystemState state)
        {
            var harvesteeHealthMap = new HarvesteeHealthMap
            {
                Value = new(100, Allocator.Persistent),
            };

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(harvesteeHealthMap);
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
        private void InitCurrentHp(
            in Entity harvesteeEntity
            , ref HarvesteeHealthId harvesteeHealthId
            , uint maxHp)
        {
            var harvesteeHealthMap = SystemAPI.GetSingleton<HarvesteeHealthMap>();

            var healthId = new HealthId
            {
                Index = harvesteeEntity.Index,
                Version = harvesteeEntity.Version,
            };

            if (!harvesteeHealthMap.Value.TryAdd(healthId, maxHp))
            {
                UnityEngine.Debug.LogError($"HarvesteeHealthMap already contains {healthId}");
                return;
            }

            harvesteeHealthId.Value = healthId;
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
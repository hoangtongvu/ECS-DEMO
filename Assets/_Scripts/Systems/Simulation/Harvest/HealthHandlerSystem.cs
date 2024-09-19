using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Unity.Collections;
using Utilities;
using Components.MyEntity.EntitySpawning;
using Core.Harvest;

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
                    , NewlySpawnedTag>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var harvesteeHealthMap = SystemAPI.GetSingleton<HarvesteeHealthMap>();

            foreach (var (profileIdRef, healthIdRef, harvesteeEntity) in
                SystemAPI.Query<
                    RefRO<HarvesteeProfileIdHolder>
                    , RefRW<HarvesteeHealthId>>()
                    .WithAll<NewlySpawnedTag>()
                    .WithEntityAccess())
            {

                uint maxHp = this.GetMaxHp(in profileIdRef.ValueRO.Value);

                var healthId = new HealthId
                {
                    Index = harvesteeEntity.Index,
                    Version = harvesteeEntity.Version,
                };


                if (!harvesteeHealthMap.Value.TryAdd(healthId, maxHp))
                {
                    UnityEngine.Debug.LogError($"HarvesteeHealthMap already contains {healthId}");
                    continue;
                }

                healthIdRef.ValueRW.Value = healthId;

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
        private uint GetMaxHp(in HarvesteeProfileId harvesteeProfileId)
        {
            var harvesteeProfileMap = SystemAPI.GetSingleton<HarvesteeProfileMap>();

            if (!harvesteeProfileMap.Value.TryGetValue(harvesteeProfileId, out var harvesteeProfile))
            {
                UnityEngine.Debug.LogError($"HarvesteeProfileMap does not contain {harvesteeProfileId}");
                return 0;
            }

            return harvesteeProfile.MaxHp;
        }

    }
}
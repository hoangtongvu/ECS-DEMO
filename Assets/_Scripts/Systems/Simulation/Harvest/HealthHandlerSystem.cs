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
                    HarvesteeHealthId
                    , NewlySpawnedTag>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var harvesteeHealthMap = SystemAPI.GetSingleton<HarvesteeHealthMap>();

            foreach (var (healthIdRef, harvesteeEntity) in
                SystemAPI.Query<
                    RefRW<HarvesteeHealthId>>()
                    .WithAll<NewlySpawnedTag>()
                    .WithEntityAccess())
            {
                var healthId = new HealthId
                {
                    Index = harvesteeEntity.Index,
                    Version = harvesteeEntity.Version,
                };

                const uint hp = 100; //Find another way to get this.

                if (!harvesteeHealthMap.Value.TryAdd(healthId, hp))
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

    }
}
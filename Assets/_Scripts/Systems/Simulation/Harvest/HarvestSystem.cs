using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Core.Harvest;

namespace Systems.Simulation.Harvest
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct HarvestSystem : ISystem
    {
        private float counterSecond;
        private float maxSecond;
        private float countSpeed;
        private uint dmg;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    HarvestTargetEntity>()
                .Build();

            state.RequireForUpdate(query0);

            this.counterSecond = 0;
            this.maxSecond = 1;
            this.countSpeed = 0.5f;
            this.dmg = 5;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var harvesteeHealthMap = SystemAPI.GetSingleton<HarvesteeHealthMap>();

            foreach (var harvestTargetRef in
                SystemAPI.Query<
                    RefRO<HarvestTargetEntity>>())
            {
                var harvestEntity = harvestTargetRef.ValueRO.Value;
                
                bool noHarvestTargetFound = harvestEntity == Entity.Null;
                if (noHarvestTargetFound) continue;

                this.counterSecond += this.countSpeed * SystemAPI.Time.DeltaTime;
                if (counterSecond < maxSecond)
                {
                    continue;
                }

                var healthId = new HealthId
                {
                    Index = harvestEntity.Index,
                    Version = harvestEntity.Version,
                };


                this.counterSecond = 0;
                if (!harvesteeHealthMap.Value.TryGetValue(healthId, out var healthValue))
                {
                    UnityEngine.Debug.LogError($"HarvesteeHealthMap does not contain {healthId}");
                    continue;
                }


                harvesteeHealthMap.Value[healthId] = healthValue <= this.dmg ? 0 : healthValue - this.dmg;

                SystemAPI.SetComponentEnabled<HarvesteeHealthChangedTag>(harvestEntity, true);

                UnityEngine.Debug.Log($"CurrHp = {harvesteeHealthMap.Value[healthId]}");

            }

        }



    }
}
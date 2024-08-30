using Unity.Entities;
using Unity.Burst;
using Components.Harvest;

namespace Systems.Simulation.Harvest
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(HarvestSystem))]
    [BurstCompile]
    public partial struct HealthChangedTagClearSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    HarvesteeHealthChangedTag>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var harvesteeHealthChangedRef in
                SystemAPI.Query<EnabledRefRW<HarvesteeHealthChangedTag>>())
            {
                harvesteeHealthChangedRef.ValueRW = false;
            }
        }



    }
}
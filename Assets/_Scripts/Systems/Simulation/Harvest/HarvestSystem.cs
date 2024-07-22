using Unity.Entities;
using Unity.Burst;
using Components.MyEntity;
using Systems.Simulation.MyEntity;
using Components.Harvest;

namespace Systems.Simulation.Harvest
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SetCanInteractFlagSystem))]
    [BurstCompile]
    public partial struct HarvestSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    TargetEntity
                    , CanInteractEntityTag>()
                .Build();

            state.RequireForUpdate(query0);

            state.RequireForUpdate<HarvesteeTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var targetEntityRef in
                SystemAPI.Query<
                    RefRO<TargetEntity>>()
                    .WithAll<CanInteractEntityTag>())
            {
                if (!SystemAPI.HasComponent<HarvesteeTag>(targetEntityRef.ValueRO.Value)) continue;
                UnityEngine.Debug.Log($"Interacted {targetEntityRef.ValueRO.Value}.");
            }

        }



    }
}
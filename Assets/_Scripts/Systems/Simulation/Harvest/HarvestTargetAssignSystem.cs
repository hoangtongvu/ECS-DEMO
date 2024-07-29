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
    public partial struct HarvestTargetAssignSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    TargetEntity
                    , HarvestTargetEntity
                    , CanInteractEntityTag>()
                .Build();

            state.RequireForUpdate(query0);

            state.RequireForUpdate<HarvesteeTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (targetEntityRef, harvestTargetRef) in
                SystemAPI.Query<
                    RefRO<TargetEntity>
                    , RefRW<HarvestTargetEntity>>()
                    .WithAll<
                        CanInteractEntityTag>())
            {
                var targetEntity = targetEntityRef.ValueRO.Value;

                if (!SystemAPI.HasComponent<HarvesteeTag>(targetEntity)) continue;

                harvestTargetRef.ValueRW.Value = targetEntity;

            }

        }

    }
}
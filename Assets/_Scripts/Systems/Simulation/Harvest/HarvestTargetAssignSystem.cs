using Unity.Entities;
using Unity.Burst;
using Systems.Simulation.GameEntity;
using Components.Harvest;
using Components.Unit;
using Core.GameEntity;
using Components.GameEntity.Interaction;

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
                    , InteractingEntity
                    , InteractionTypeICD
                    , CanInteractEntityTag>()
                .Build();

            state.RequireForUpdate(query0);

            state.RequireForUpdate<HarvesteeTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (targetEntityRef, interactingEntityRef, interactionTypeICDRef, harvesteeTypeRef) in
                SystemAPI.Query<
                    RefRO<TargetEntity>
                    , RefRW<InteractingEntity>
                    , RefRW<InteractionTypeICD>
                    , RefRW<HarvesteeTypeHolder>>()
                    .WithAll<
                        CanInteractEntityTag>())
            {
                var targetEntity = targetEntityRef.ValueRO.Value;

                if (!SystemAPI.HasComponent<HarvesteeTag>(targetEntity)) continue;

                interactingEntityRef.ValueRW.Value = targetEntity;
                interactionTypeICDRef.ValueRW.Value = InteractionType.Harvest;

                var harvesteeProfileIdRef = SystemAPI.GetComponentRO<HarvesteeProfileIdHolder>(targetEntity);

                harvesteeTypeRef.ValueRW.Value = harvesteeProfileIdRef.ValueRO.Value.HarvesteeType;
            }

        }

    }

}
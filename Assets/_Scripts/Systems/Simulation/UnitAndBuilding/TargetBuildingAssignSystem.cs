using Unity.Entities;
using Unity.Burst;
using Systems.Simulation.GameEntity;
using Core.GameEntity;
using Components.GameEntity.Interaction;
using Components.Misc.WorldMap.WorldBuilding.Blueprint;

namespace Systems.Simulation.UnitAndBuilding
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SetCanInteractFlagSystem))]
    [BurstCompile]
    public partial struct TargetBuildingAssignSystem : ISystem
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

            state.RequireForUpdate<ConstructionRemaining>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (targetEntityRef, interactingEntityRef, interactionTypeICDRef) in
                SystemAPI.Query<
                    RefRO<TargetEntity>
                    , RefRW<InteractingEntity>
                    , RefRW<InteractionTypeICD>>()
                    .WithAll<
                        CanInteractEntityTag>())
            {
                var targetEntity = targetEntityRef.ValueRO.Value;

                if (!SystemAPI.HasComponent<ConstructionRemaining>(targetEntity)) continue;

                interactingEntityRef.ValueRW.Value = targetEntity;
                interactionTypeICDRef.ValueRW.Value = InteractionType.Attack; // TODO: Change this

            }

        }

    }

}
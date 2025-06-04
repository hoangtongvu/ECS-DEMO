using Unity.Entities;
using Unity.Burst;
using Systems.Simulation.GameEntity;
using Core.GameEntity;
using Components.GameEntity.Misc;
using Components.GameEntity.Interaction;

namespace Systems.Simulation.Unit.Misc
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SetCanInteractFlagSystem))]
    [BurstCompile]
    public partial struct AttackTargetAssignSystem : ISystem
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

            state.RequireForUpdate<ArmedStateHolder>();
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

                if (!SystemAPI.HasComponent<ArmedStateHolder>(targetEntity)) continue;

                interactingEntityRef.ValueRW.Value = targetEntity;
                interactionTypeICDRef.ValueRW.Value = InteractionType.Attack;

            }

        }

    }

}
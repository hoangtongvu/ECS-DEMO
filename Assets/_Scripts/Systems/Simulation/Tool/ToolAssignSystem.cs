using Unity.Entities;
using Unity.Burst;
using Systems.Simulation.GameEntity;
using Components.Tool;
using Components.GameEntity.Interaction;

namespace Systems.Simulation.Tool
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SetCanInteractFlagSystem))]
    [BurstCompile]
    public partial struct ToolAssignSystem : ISystem
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

            state.RequireForUpdate<DerelictToolTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (targetEntityRef, unitEntity) in
                SystemAPI.Query<
                    RefRO<TargetEntity>>()
                    .WithAll<CanInteractEntityTag>()
                    .WithEntityAccess())
            {
                var targetEntity = targetEntityRef.ValueRO.Value;

                bool targetEntityIsTool = SystemAPI.HasComponent<DerelictToolTag>(targetEntity);
                if (!targetEntityIsTool) continue;

                bool targetEntityIsDerelictTool = SystemAPI.IsComponentEnabled<DerelictToolTag>(targetEntity);
                if (!targetEntityIsDerelictTool) continue;

                bool toolMarkedAsCanBePicked = SystemAPI.IsComponentEnabled<CanBePickedTag>(targetEntity);
                if (toolMarkedAsCanBePicked) continue;

                SystemAPI.SetComponentEnabled<CanBePickedTag>(targetEntity, true);

                toolMarkedAsCanBePicked = SystemAPI.IsComponentEnabled<CanBePickedTag>(targetEntity);

                var toolPickerEntityRef = SystemAPI.GetComponentRW<ToolPickerEntity>(targetEntity);
                toolPickerEntityRef.ValueRW.Value = unitEntity;
            }

        }

    }

}
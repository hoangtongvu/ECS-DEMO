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

            var query1 = SystemAPI.QueryBuilder()
                .WithAll<
                    DerelictToolTag
                    , CanBePickedTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate(query1);
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

                bool targetEntityIsDerelictTool = SystemAPI.HasComponent<DerelictToolTag>(targetEntity);
                if (!targetEntityIsDerelictTool) continue;

                bool toolMarkedAsCanBePicked = SystemAPI.IsComponentEnabled<CanBePickedTag>(targetEntity);
                if (toolMarkedAsCanBePicked) continue;

                this.MarkToolCanBePicked(ref state, in targetEntity, in unitEntity);
            }

        }

        [BurstCompile]
        private void MarkToolCanBePicked(ref SystemState state, in Entity toolEntity, in Entity unitEntity)
        {
            SystemAPI.SetComponentEnabled<CanBePickedTag>(toolEntity, true);

            var toolPickerEntityRef = SystemAPI.GetComponentRW<ToolPickerEntity>(toolEntity);
            toolPickerEntityRef.ValueRW.Value = unitEntity;
        }

    }

}
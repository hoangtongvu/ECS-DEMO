using Components.GameEntity.Interaction;
using Components.Tool;
using Components.Tool.Picker;
using Components.Unit;
using Systems.Simulation.GameEntity;
using Unity.Burst;
using Unity.Entities;

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
                    , CanInteractEntityTag
                    , CanPickToolTag
                    , ToolToPick>()
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
            foreach (var (targetEntityRef, unitToolHolderRef, unitEntity) in SystemAPI
                .Query<
                    RefRO<TargetEntity>
                    , RefRO<UnitToolHolder>>()
                .WithAll<CanInteractEntityTag>()
                .WithDisabled<CanPickToolTag>()
                .WithEntityAccess())
            {
                bool toolSlotIsEmpty = unitToolHolderRef.ValueRO.Value == Entity.Null;
                if (!toolSlotIsEmpty) continue;

                var targetEntity = targetEntityRef.ValueRO.Value;

                bool targetEntityIsDerelictTool = SystemAPI.HasComponent<DerelictToolTag>(targetEntity);
                if (!targetEntityIsDerelictTool) continue;

                bool toolMarkedAsCanBePicked = SystemAPI.IsComponentEnabled<CanBePickedTag>(targetEntity);
                if (toolMarkedAsCanBePicked) continue;

                this.MarkToolCanBePicked(ref state, in targetEntity, in unitEntity);
                this.MarkUnitCanPickTool(ref state, in targetEntity, in unitEntity);
            }

        }

        [BurstCompile]
        private void MarkToolCanBePicked(ref SystemState state, in Entity toolEntity, in Entity unitEntity)
        {
            SystemAPI.SetComponentEnabled<CanBePickedTag>(toolEntity, true);
            SystemAPI.SetComponent(toolEntity, new ToolPickerEntity
            {
                Value = unitEntity,
            });
        }

        [BurstCompile]
        private void MarkUnitCanPickTool(ref SystemState state, in Entity toolEntity, in Entity unitEntity)
        {
            SystemAPI.SetComponentEnabled<CanPickToolTag>(unitEntity, true);
            SystemAPI.SetComponent(unitEntity, new ToolToPick(toolEntity));
        }

    }

}
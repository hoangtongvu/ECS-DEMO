using Unity.Entities;
using Unity.Burst;
using Components.Tool;
using Components.Unit;
using Core.Tool;
using Core.Unit;
using Components.Harvest;
using Unity.Transforms;
using Unity.Physics;
using Unity.Mathematics;
using Core.Utilities.Extensions;
using Components.Misc;
using Components.GameEntity.EntitySpawning;

namespace Systems.Simulation.Tool
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ToolAssignSystem))]
    [BurstCompile]
    public partial struct ToolPickSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    SpawnerEntityRef
                    , CanBePickedTag
                    , ToolPickerEntity
                    , DerelictToolTag>()
                .Build();

            state.RequireForUpdate(query0);

            state.RequireForUpdate<UnitToolHolder>();
            state.RequireForUpdate<ToolHoldCount>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var tool2UnitMap = SystemAPI.GetSingleton<Tool2UnitMap>();
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (transformRef, spawnerEntityRef, toolTypeICDRef, canBePickedTagRef, toolPickerEntityRef, dmgRef, speedRef, toolEntity) in
                SystemAPI.Query<
                    RefRW<LocalTransform>
                    , RefRW<SpawnerEntityRef>
                    , RefRO<ToolTypeICD>
                    , EnabledRefRW<CanBePickedTag>
                    , RefRW<ToolPickerEntity>
                    , RefRO<BaseDmg>
                    , RefRO<BaseWorkSpeed>>()
                    .WithEntityAccess()
                    .WithAll<DerelictToolTag>())
            {
                this.HandleUnit(
                    ref state
                    , ecb
                    , toolPickerEntityRef.ValueRO.Value
                    , toolEntity
                    , tool2UnitMap
                    , toolTypeICDRef.ValueRO.Value
                    , dmgRef.ValueRO.Value
                    , speedRef.ValueRO.Value);

                this.HandleToolSpawner(
                    ref state
                    , ref spawnerEntityRef.ValueRW);

                this.HandleTool(
                    ref state
                    , transformRef
                    , ecb
                    , canBePickedTagRef
                    , toolPickerEntityRef
                    , toolEntity);

            }

        }

        [BurstCompile]
        private void HandleToolSpawner(
            ref SystemState state
            , ref SpawnerEntityRef spawnerEntityRef)
        {
            var toolHoldCountRef = SystemAPI.GetComponentRW<ToolHoldCount>(spawnerEntityRef.Value);
            toolHoldCountRef.ValueRW.Value--;
            spawnerEntityRef.Value = Entity.Null;
        }

        [BurstCompile]
        private void HandleTool(
            ref SystemState state
            , RefRW<LocalTransform> transformRef
            , EntityCommandBuffer ecb
            , EnabledRefRW<CanBePickedTag> canBePickedTagRef
            , RefRW<ToolPickerEntity> toolPickerEntityRef
            , Entity toolEntity)
        {
            // Make unit become tool's parent
            ecb.AddComponent(toolEntity, new Parent
            {
                Value = toolPickerEntityRef.ValueRO.Value
            });

            // Remove Gravity of tool
            ecb.AddComponent(toolEntity, new PhysicsGravityFactor
            {
                Value = 0
            });

            // Set temp offset for tool (stay on unit's head)
            var unitTransform = SystemAPI.GetComponentRO<LocalTransform>(toolPickerEntityRef.ValueRO.Value);
            transformRef.ValueRW.Position = unitTransform.ValueRO.Position.Add(y: 4f);
            transformRef.ValueRW.Scale *= 2;
            transformRef.ValueRW.Rotation = quaternion.identity;


            SystemAPI.SetComponentEnabled<DerelictToolTag>(toolEntity, false);
            canBePickedTagRef.ValueRW = false;
            toolPickerEntityRef.ValueRW.Value = Entity.Null;
        }

        [BurstCompile]
        private void HandleUnit(
            ref SystemState state
            , EntityCommandBuffer ecb
            , in Entity unitEntity
            , in Entity toolEntity
            , in Tool2UnitMap tool2UnitMap
            , in ToolType toolType
            , uint baseDmg
            , float baseWorkSpeed)
        {
            var unitToolHolderRef = SystemAPI.GetComponentRW<UnitToolHolder>(unitEntity);
            unitToolHolderRef.ValueRW.Value = toolEntity;

            SystemAPI.SetComponentEnabled<JoblessUnitTag>(unitEntity, false);

            var toolTypeRef = SystemAPI.GetComponentRW<ToolTypeICD>(unitEntity);
            toolTypeRef.ValueRW.Value = toolType;
            var baseDmgRef = SystemAPI.GetComponentRW<BaseDmg>(unitEntity);
            baseDmgRef.ValueRW.Value = baseDmg;
            var baseWorkSpeedRef = SystemAPI.GetComponentRW<BaseWorkSpeed>(unitEntity);
            baseWorkSpeedRef.ValueRW.Value = baseWorkSpeed;

            var unitProfileIdHolderRef = SystemAPI.GetComponentRW<UnitProfileIdHolder>(unitEntity);

            var byteKey = (byte)toolType;
            if (tool2UnitMap.Value.TryGetValue(byteKey, out byte unitTypeByte))
            {
                var unitType = (UnitType)unitTypeByte;

                switch (unitType)
                {
                    case UnitType.None:
                        
                        break;
                    case UnitType.Villager:
                        
                        break;
                    case UnitType.Knight:
                        
                        break;
                    case UnitType.Harvester:
                        ecb.AddComponent<HarvesterICD>(unitEntity);
                        ecb.AddComponent<HarvesteeTypeHolder>(unitEntity);
                        break;
                }

                unitProfileIdHolderRef.ValueRW.Value.UnitType = unitType;
                return;
            }

            UnityEngine.Debug.LogError($"tool2UnitMap does not contain {byteKey}");

        }

    }

}
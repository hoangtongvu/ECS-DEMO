using Unity.Entities;
using Unity.Burst;
using Components.Tool;
using Components.Unit;
using Components.MyEntity.EntitySpawning;
using Core.Tool;
using Core.Unit;
using Components.Harvest;

namespace Systems.Simulation.Tool
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ToolCallerSystem))]
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

            foreach (var (spawnerEntityRef, toolTypeICDRef, canBePickedTagRef, toolPickerEntityRef, toolEntity) in
                SystemAPI.Query<
                    RefRW<SpawnerEntityRef>
                    , RefRO<ToolTypeICD>
                    , EnabledRefRW<CanBePickedTag>
                    , RefRW<ToolPickerEntity>>()
                    .WithEntityAccess()
                    .WithAll<DerelictToolTag>())
            {
                this.HandleUnit(
                    ref state
                    , ecb
                    , toolPickerEntityRef.ValueRO.Value
                    , toolEntity
                    , tool2UnitMap
                    , toolTypeICDRef.ValueRO.Value);

                //TODO: Add ToolSpawnerHandler() as the way we did with UnitHandler().
                var toolHoldCountRef = SystemAPI.GetComponentRW<ToolHoldCount>(spawnerEntityRef.ValueRO.Value);

                SystemAPI.SetComponentEnabled<DerelictToolTag>(toolEntity, false);

                toolHoldCountRef.ValueRW.Value--;
                spawnerEntityRef.ValueRW.Value = Entity.Null;

                // Reset ToolPickHandler
                canBePickedTagRef.ValueRW = false;
                toolPickerEntityRef.ValueRW.Value = Entity.Null;

            }


        }

        [BurstCompile]
        private void HandleUnit(
            ref SystemState state
            , EntityCommandBuffer ecb
            , in Entity unitEntity
            , in Entity toolEntity
            , in Tool2UnitMap tool2UnitMap
            , in ToolType toolType)
        {
            var unitToolHolderRef = SystemAPI.GetComponentRW<UnitToolHolder>(unitEntity);
            unitToolHolderRef.ValueRW.Value = toolEntity;


            var unitIdRef = SystemAPI.GetComponentRW<UnitId>(unitEntity);

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
                        ecb.AddComponent<HarvestTargetEntity>(unitEntity);
                        break;
                }

                unitIdRef.ValueRW.UnitType = unitType;
                return;
            }

            UnityEngine.Debug.LogError($"tool2UnitMap does not contain {byteKey}");
        }

    }
}
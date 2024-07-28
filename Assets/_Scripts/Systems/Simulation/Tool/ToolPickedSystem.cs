using Unity.Entities;
using Unity.Burst;
using Components.Tool;
using Components.Unit;
using Components.MyEntity.EntitySpawning;
using Core.Tool;
using Core.Unit;

namespace Systems.Simulation.Tool
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ToolCallerSystem))]
    [BurstCompile]
    public partial struct ToolPickedSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    SpawnerEntityRef
                    , CanBePicked
                    , PickedBy
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

            foreach (var (spawnerEntityRef, toolTypeICDRef, canBePickedRef, pickedByRef, toolEntity) in
                SystemAPI.Query<
                    RefRW<SpawnerEntityRef>
                    , RefRO<ToolTypeICD>
                    , RefRW<CanBePicked> //TODO Turn this into EnableAble tag.
                    , RefRW<PickedBy>>()
                    .WithEntityAccess()
                    .WithAll<DerelictToolTag>())
            {
                if (!canBePickedRef.ValueRO.Value) continue;

                this.UnitHandler(
                    ref state
                    , pickedByRef.ValueRO.Value
                    , toolEntity
                    , tool2UnitMap
                    , toolTypeICDRef.ValueRO.Value);

                //TODO: Add ToolSpawnerHandler() as the way we did with UnitHandler().
                var toolHoldCountRef = SystemAPI.GetComponentRW<ToolHoldCount>(spawnerEntityRef.ValueRO.Value);

                SystemAPI.SetComponentEnabled<DerelictToolTag>(toolEntity, false);

                toolHoldCountRef.ValueRW.Value--;
                spawnerEntityRef.ValueRW.Value = Entity.Null;

                // Reset ToolPickHandler
                canBePickedRef.ValueRW.Value = false;
                pickedByRef.ValueRW.Value = Entity.Null;

            }


        }

        [BurstCompile]
        private void UnitHandler(
            ref SystemState state
            , in Entity unitEntity
            , in Entity toolEntity
            , in Tool2UnitMap tool2UnitMap
            , in ToolType toolType)
        {
            var unitToolHolderRef = SystemAPI.GetComponentRW<UnitToolHolder>(unitEntity);
            unitToolHolderRef.ValueRW.Value = toolEntity;


            var unitIdRef = SystemAPI.GetComponentRW<UnitId>(unitEntity);

            var byteKey = (byte)toolType;
            if (tool2UnitMap.Value.TryGetValue(byteKey, out byte unitType))
            {
                unitIdRef.ValueRW.UnitType = (UnitType)unitType;
                return;
            }

            UnityEngine.Debug.LogError($"tool2UnitMap does not contain {byteKey}");
        }

    }
}
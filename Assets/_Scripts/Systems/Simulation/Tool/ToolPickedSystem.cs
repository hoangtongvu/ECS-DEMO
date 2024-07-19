using Unity.Entities;
using Unity.Burst;
using Components.Tool;
using Components.Unit;
using Components.MyEntity.EntitySpawning;

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

            foreach (var (spawnerEntityRef, canBePickedRef, pickedByRef, toolEntity) in
                SystemAPI.Query<
                    RefRW<SpawnerEntityRef>
                    , RefRW<CanBePicked>
                    , RefRW<PickedBy>>()
                    .WithEntityAccess()
                    .WithAll<DerelictToolTag>())
            {
                if (!canBePickedRef.ValueRO.Value) continue;

                var unitToolHolderRef = SystemAPI.GetComponentRW<UnitToolHolder>(pickedByRef.ValueRO.Value);
                var toolHoldCountRef = SystemAPI.GetComponentRW<ToolHoldCount>(spawnerEntityRef.ValueRO.Value);

                // Pick the tool
                unitToolHolderRef.ValueRW.Value = toolEntity;

                SystemAPI.SetComponentEnabled<DerelictToolTag>(toolEntity, false);

                toolHoldCountRef.ValueRW.Value--;
                spawnerEntityRef.ValueRW.Value = Entity.Null;

                // Reset ToolPickHandler
                canBePickedRef.ValueRW.Value = false;
                pickedByRef.ValueRW.Value = Entity.Null;

            }


        }

        

    }
}
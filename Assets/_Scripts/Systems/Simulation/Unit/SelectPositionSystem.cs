using Unity.Entities;
using Components.Unit;
using Core;
using Components;
using Unity.Burst.CompilerServices;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RaycastHitSelectionSystem))]
    [BurstCompile]
    public partial struct SelectPositionSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectionHitData>();
            state.RequireForUpdate<SelectedUnitElement>();
            state.RequireForUpdate<MoveableState>();
            state.RequireForUpdate<TargetPosition>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Checking Hit Data.
            var selectionHitRef = SystemAPI.GetSingletonRW<SelectionHitData>();
            if (!selectionHitRef.ValueRO.NewlyAdded) return;
            if (selectionHitRef.ValueRO.SelectionType != SelectionType.Position) return;
            selectionHitRef.ValueRW.NewlyAdded = false;

            // Set Units move to target Position.
            var selectedUnits = SystemAPI.GetSingletonBuffer<SelectedUnitElement>();

            var job = new SetTargetJob
            {
                selectedUnits = selectedUnits,
                targetPosition = selectionHitRef.ValueRO.RaycastHit.Position,
                targetPosLookup = SystemAPI.GetComponentLookup<TargetPosition>(),
                moveableStateLookup = SystemAPI.GetComponentLookup<MoveableState>(),
            };

            state.Dependency = default;
            state.Dependency = job.ScheduleParallelByRef(selectedUnits.Length, 32, state.Dependency);

        }

        [BurstCompile]
        private struct SetTargetJob : IJobParallelForBatch
        {
            [NativeDisableParallelForRestriction]
            public DynamicBuffer<SelectedUnitElement> selectedUnits;

            public float3 targetPosition;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<TargetPosition> targetPosLookup;
            
            [NativeDisableParallelForRestriction]
            public ComponentLookup<MoveableState> moveableStateLookup;

            public void Execute(int startIndex, int count)
            {
                var length = startIndex + count;

                for (int i = startIndex; i < length; i++)
                {
                    Entity entity = selectedUnits.ElementAt(i).Value;

                    if (Hint.Likely(!this.moveableStateLookup.HasComponent(entity))) continue;
                    this.moveableStateLookup.SetComponentEnabled(entity, true);

                    var targetPosRef = this.targetPosLookup.GetRefRWOptional(entity);
                    targetPosRef.ValueRW.Value = this.targetPosition;
                }
            }
        }


    }
}
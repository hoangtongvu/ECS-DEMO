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

            // Set all selected units Moveable.
            // This can't be put in job cause SystemAPI.SetComponentEnabled available in SystemAPI only.
            var selectedUnits = SystemAPI.GetSingletonBuffer<SelectedUnitElement>();
            foreach (var unit in selectedUnits)
            {
                if (Hint.Likely(!SystemAPI.HasComponent<MoveableState>(unit.Value))) continue;
                SystemAPI.SetComponentEnabled<MoveableState>(unit.Value, true);
            }

            var job = new SetTargetJob
            {
                selectedUnits = selectedUnits,
                targetPosition = selectionHitRef.ValueRO.RaycastHit.Position,
                targetPosLookup = SystemAPI.GetComponentLookup<TargetPosition>(),
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

            public void Execute(int startIndex, int count)
            {
                var length = startIndex + count;

                for (int i = startIndex; i < length; i++)
                {
                    var targetPosRef = this.targetPosLookup.GetRefRWOptional(selectedUnits.ElementAt(i).Value);
                    targetPosRef.ValueRW.Value = this.targetPosition;
                }
            }
        }


    }
}
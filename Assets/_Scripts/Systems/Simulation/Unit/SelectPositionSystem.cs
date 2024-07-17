using Unity.Entities;
using Components.Unit;
using Core;
using Components;
using Unity.Burst.CompilerServices;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Core.Unit;
using Utilities.Helpers;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DragSelectionSystem))]
    [BurstCompile]
    public partial struct SelectPositionSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectionHitElement>();
            state.RequireForUpdate<SelectedUnitElement>();
            state.RequireForUpdate<MoveableState>();
            state.RequireForUpdate<TargetPosition>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveAffecterMap = SystemAPI.GetSingleton<MoveAffecterMap>();

            // Checking Hit Data.
            if (!this.TryGetSelectedPosition(out float3 selectedPos)) return;

            // Set Units move to target Position.
            var selectedUnits = SystemAPI.GetSingletonBuffer<SelectedUnitElement>();

            var job = new SetTargetJob
            {
                selectedUnits = selectedUnits,
                targetPosition = selectedPos,
                targetPosLookup = SystemAPI.GetComponentLookup<TargetPosition>(),
                moveableStateLookup = SystemAPI.GetComponentLookup<MoveableState>(),
                moveAffecterLookup = SystemAPI.GetComponentLookup<MoveAffecterICD>(),
                unitIdLookup = SystemAPI.GetComponentLookup<UnitId>(),
                moveAffecterMap = moveAffecterMap.Value,
            };

            state.Dependency = default;
            state.Dependency = job.ScheduleParallelByRef(selectedUnits.Length, 32, state.Dependency);

        }

        private bool TryGetSelectedPosition(out float3 selectedPos)
        {
            selectedPos = float3.zero;

            var selectionHits = SystemAPI.GetSingletonBuffer<SelectionHitElement>();
            if (selectionHits.IsEmpty) return false;

            for (int i = 0; i < selectionHits.Length; i++)
            {
                var hit = selectionHits[i];
                if (hit.SelectionType != SelectionType.Position) continue;
                selectedPos = hit.HitPos;
                selectionHits.RemoveAt(i);
                i--; // Unnecessary line.
                return true;
            }

            return false;
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

            [NativeDisableParallelForRestriction]
            public ComponentLookup<MoveAffecterICD> moveAffecterLookup;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<UnitId> unitIdLookup;

            [ReadOnly]
            public NativeHashMap<MoveAffecterId, byte> moveAffecterMap;

            public void Execute(int startIndex, int count)
            {
                var length = startIndex + count;

                for (int i = startIndex; i < length; i++)
                {
                    Entity entity = selectedUnits.ElementAt(i).Value;

                    if (Hint.Likely(!this.moveableStateLookup.HasComponent(entity))) continue;


                    var moveAffecterRef = this.moveAffecterLookup.GetRefRWOptional(entity);
                    var unitIdRef = this.unitIdLookup.GetRefROOptional(entity);


                    if (!MoveAffecterHelper.TryChangeMoveAffecter(
                        in this.moveAffecterMap
                        , unitIdRef.ValueRO.UnitType
                        , ref moveAffecterRef.ValueRW
                        , MoveAffecter.PlayerCommand
                        , unitIdRef.ValueRO.LocalIndex))
                    {
                        continue;
                    }


                    this.moveableStateLookup.SetComponentEnabled(entity, true);

                    var targetPosRef = this.targetPosLookup.GetRefRWOptional(entity);
                    targetPosRef.ValueRW.Value = this.targetPosition;

                }
            }
        }


    }
}
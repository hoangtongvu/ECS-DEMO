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
using Components.MyEntity;
using Unity.Transforms;

namespace Systems.Simulation.MyEntity
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DragSelectionSystem))]
    [BurstCompile]
    public partial struct SelectInteractableEntitySystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectionHitElement>();
            state.RequireForUpdate<SelectedUnitElement>();
            state.RequireForUpdate<MoveableState>();
            state.RequireForUpdate<TargetPosition>();
            state.RequireForUpdate<InteractableEntityTag>();
            state.RequireForUpdate<TargetEntity>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveAffecterMap = SystemAPI.GetSingleton<MoveAffecterMap>();

            // Checking Hit Data.
            if (!this.TryGetInteractable(out Entity entity, out float3 pos)) return;

            // Set Units move to target Position.
            var selectedUnits = SystemAPI.GetSingletonBuffer<SelectedUnitElement>();

            var job = new SetTargetJob
            {
                selectedUnits = selectedUnits,
                targetEntity = entity,
                targetPosition = pos,
                targetPosLookup = SystemAPI.GetComponentLookup<TargetPosition>(),
                distanceToTargetLookup = SystemAPI.GetComponentLookup<DistanceToTarget>(),
                transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                moveableStateLookup = SystemAPI.GetComponentLookup<MoveableState>(),
                moveAffecterLookup = SystemAPI.GetComponentLookup<MoveAffecterICD>(),
                unitIdLookup = SystemAPI.GetComponentLookup<UnitId>(),
                targetEntityLookup = SystemAPI.GetComponentLookup<TargetEntity>(),
                moveAffecterMap = moveAffecterMap.Value,
            };

            state.Dependency = default;
            state.Dependency = job.ScheduleParallelByRef(selectedUnits.Length, 32, state.Dependency);

        }

        [BurstCompile]
        private bool TryGetInteractable(out Entity entity, out float3 pos)
        {
            entity = Entity.Null;
            pos = float3.zero;

            var selectionHits = SystemAPI.GetSingletonBuffer<SelectionHitElement>();
            if (selectionHits.IsEmpty) return false;
            int length = selectionHits.Length;

            for (int i = 0; i < length; i++)
            {
                var hit = selectionHits[i];
                if (hit.SelectionType != SelectionType.InteractableEntity) continue;

                entity = hit.HitEntity;
                pos = hit.HitPos;

                selectionHits.RemoveAt(i);
                return true;
            }

            return false;
        }



        [BurstCompile]
        private struct SetTargetJob : IJobParallelForBatch // Duplicated Job, same as one in SelectPosSystem.
        {
            [NativeDisableParallelForRestriction]
            public DynamicBuffer<SelectedUnitElement> selectedUnits;

            public Entity targetEntity;
            public float3 targetPosition;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<TargetPosition> targetPosLookup;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<DistanceToTarget> distanceToTargetLookup;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<LocalTransform> transformLookup;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<MoveableState> moveableStateLookup;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<MoveAffecterICD> moveAffecterLookup;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<UnitId> unitIdLookup;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<TargetEntity> targetEntityLookup;

            [ReadOnly]
            public NativeHashMap<MoveAffecterId, byte> moveAffecterMap;

            [BurstCompile]
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

                    // Init distance to target.
                    var distanceToTargetRef = this.distanceToTargetLookup.GetRefRWOptional(entity);
                    var transformRef = this.transformLookup.GetRefROOptional(entity);
                    distanceToTargetRef.ValueRW.CurrentDistance = MathHelper.GetDistance2(transformRef.ValueRO.Position, this.targetPosition);

                    this.moveableStateLookup.SetComponentEnabled(entity, true);

                    var targetPosRef = this.targetPosLookup.GetRefRWOptional(entity);
                    targetPosRef.ValueRW.Value = this.targetPosition;

                    var targetEntityRef = this.targetEntityLookup.GetRefRWOptional(entity);
                    targetEntityRef.ValueRW.Value = this.targetEntity;
                }
            }
        }


    }
}
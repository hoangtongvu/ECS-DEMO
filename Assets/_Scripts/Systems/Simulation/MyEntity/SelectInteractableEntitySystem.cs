using Unity.Entities;
using Components.Unit;
using Core;
using Components;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Core.Unit;
using Utilities.Helpers;
using Components.MyEntity;
using Unity.Transforms;
using Components.Unit.UnitSelection;

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
            state.RequireForUpdate<InteractableEntityTag>();
            state.RequireForUpdate<SelectionHitElement>();
            state.RequireForUpdate<MoveAffecterMap>();

            EntityQuery query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitSelectedTag
                    , MoveableState
                    , LocalTransform
                    , DistanceToTarget
                    , TargetEntity
                    , TargetPosition
                    , MoveAffecterICD>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveAffecterMap = SystemAPI.GetSingleton<MoveAffecterMap>();

            // Checking Hit Data.
            if (!this.TryGetInteractable(out Entity entity, out float3 pos)) return;

            new SetTargetJob()
            {
                targetEntity = entity,
                targetPosition = pos,
                moveAffecterMap = moveAffecterMap.Value,
            }.ScheduleParallel();

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


        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct SetTargetJob : IJobEntity
        {

            [ReadOnly] public Entity targetEntity;
            [ReadOnly] public float3 targetPosition;
            [ReadOnly] public NativeHashMap<MoveAffecterId, byte> moveAffecterMap;

            void Execute(
                in UnitId unitId
                , EnabledRefRO<UnitSelectedTag> unitSelectedTag
                , EnabledRefRW<MoveableState> moveableState
                , in LocalTransform transform
                , ref DistanceToTarget distanceToTarget
                , ref TargetPosition targetPosition
                , ref TargetEntity targetInteractableEntity
                , ref MoveAffecterICD moveAffecterICD)
            {
                bool unitSelected = unitSelectedTag.ValueRO;
                if (!unitSelected) return;

                if (!MoveAffecterHelper.TryChangeMoveAffecter(
                        in this.moveAffecterMap
                        , unitId.UnitType
                        , ref moveAffecterICD
                        , MoveAffecter.PlayerCommand
                        , unitId.LocalIndex))
                {
                    return;
                }

                moveableState.ValueRW = true;
                targetInteractableEntity.Value = this.targetEntity;
                targetPosition.Value = this.targetPosition;

                // Init distance to target.
                distanceToTarget.CurrentDistance = MathHelper.GetDistance2(transform.Position, this.targetPosition);

            }
        }

    }
}
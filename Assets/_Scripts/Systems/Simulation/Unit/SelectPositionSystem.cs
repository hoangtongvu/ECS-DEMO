using Unity.Entities;
using Components.Unit;
using Core;
using Components;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Core.Unit;
using Utilities.Helpers;
using Components.Unit.UnitSelection;

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
            state.RequireForUpdate<MoveAffecterMap>();

            EntityQuery query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitSelectedTag
                    , CanMoveEntityTag
                    , TargetPosition
                    , MoveAffecterICD>()
                .Build();

            state.RequireForUpdate(query);
            state.Enabled = false;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveAffecterMap = SystemAPI.GetSingleton<MoveAffecterMap>();

            // Checking Hit Data.
            if (!this.TryGetSelectedPosition(out float3 selectedPos)) return;

            new SetTargetJob()
            {
                moveAffecterMap = moveAffecterMap.Value,
                targetPosition = selectedPos,
            }.ScheduleParallel();

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


        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct SetTargetJob : IJobEntity
        {

            [ReadOnly] public float3 targetPosition;
            [ReadOnly] public NativeHashMap<MoveAffecterId, byte> moveAffecterMap;

            void Execute(
                in UnitId unitId
                , EnabledRefRO<UnitSelectedTag> unitSelectedTag
                , EnabledRefRW<CanMoveEntityTag> canMoveEntityTag
                , ref TargetPosition targetPosition
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

                canMoveEntityTag.ValueRW = true;
                targetPosition.Value = this.targetPosition;
            }
        }
    }
}
using Unity.Entities;
using Components.Unit;
using Core;
using Components;
using Unity.Mathematics;
using Unity.Burst;
using Components.Unit.UnitSelection;
using Core.Unit.MyMoveCommand;
using Components.Unit.MyMoveCommand;
using Utilities.Jobs;
using Components.Misc;
using Components.Unit.Reaction;
using Unity.Collections;
using Unity.Transforms;
using Components.Misc.WorldMap.PathFinding;
using Components.GameEntity;
using Components.Unit.Misc;

namespace Systems.Simulation.Unit
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DragSelectionSystem))]
    [BurstCompile]
    public partial struct SelectPositionSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectionHitElement>();
            state.RequireForUpdate<MoveCommandPrioritiesMap>();

            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder
                    , LocalTransform
                    , AbsoluteDistanceXZToTarget
                    , UnitSelectedTag
                    , CanFindPathTag
                    , MoveSpeedLinear
                    , TargetEntity>()
                .WithAll<
                    TargetEntityWorldSquareRadius
                    , MoveCommandElement
                    , InteractingEntity
                    , InteractionTypeICD
                    , InteractableDistanceRange
                    , ArmedStateHolder
                    , CanSetTargetJobScheduleTag>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                .Build();

            state.RequireForUpdate(this.entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Checking Hit Data.
            if (!this.TryGetSelectedPosition(out float3 selectedPos)) return;

            var moveCommandPrioritiesMap = SystemAPI.GetSingleton<MoveCommandPrioritiesMap>();
            var defaultStopMoveWorldRadius = SystemAPI.GetSingleton<DefaultStopMoveWorldRadius>().Value;
            var unitReactionConfigsMap = SystemAPI.GetSingleton<UnitReactionConfigsMap>().Value;

            var speedArray = new NativeArray<float>(this.entityQuery.CalculateEntityCount(), Allocator.TempJob);

            state.Dependency = new Set_CanSetTargetJobScheduleTag_OnUnitSelected()
                .ScheduleParallel(this.entityQuery, state.Dependency);

            state.Dependency = new GetRunSpeedsJob()
            {
                UnitReactionConfigsMap = unitReactionConfigsMap,
                OutputArray = speedArray,
            }.ScheduleParallel(this.entityQuery, state.Dependency);

            state.Dependency = new SetSingleTargetJobMultipleSpeeds()
            {
                TargetEntity = Entity.Null,
                TargetEntityWorldSquareRadius = defaultStopMoveWorldRadius,
                TargetPosition = selectedPos,
                NewMoveCommandSource = MoveCommandSource.PlayerCommand,
                MoveCommandPrioritiesMap = moveCommandPrioritiesMap,
                SpeedArray = speedArray,
            }.ScheduleParallel(this.entityQuery, state.Dependency);

            state.Dependency = new CleanUpCanSetTargetJobScheduleTagJob().ScheduleParallel(this.entityQuery, state.Dependency);

            state.Dependency = speedArray.Dispose(state.Dependency);

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

    }

}
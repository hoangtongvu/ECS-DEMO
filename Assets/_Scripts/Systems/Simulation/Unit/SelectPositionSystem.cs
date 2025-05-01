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
            state.RequireForUpdate<MoveCommandSourceMap>();

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
                    , InteractionTypeICD>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                .Build();

            state.RequireForUpdate(this.entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Checking Hit Data.
            if (!this.TryGetSelectedPosition(out float3 selectedPos)) return;

            var moveCommandSourceMap = SystemAPI.GetSingleton<MoveCommandSourceMap>();
            var defaultStopMoveWorldRadius = SystemAPI.GetSingleton<DefaultStopMoveWorldRadius>().Value;
            var unitReactionConfigsMap = SystemAPI.GetSingleton<UnitReactionConfigsMap>().Value;

            var speedArray = new NativeArray<float>(this.entityQuery.CalculateEntityCount(), Allocator.TempJob);

            var getSpeedsJobHandle = new GetRunSpeedsJob()
            {
                UnitReactionConfigsMap = unitReactionConfigsMap,
                OutputArray = speedArray,
            }.ScheduleParallel(state.Dependency);

            var setTargetJobHandle = new SetSingleTargetJobMultipleSpeeds()
            {
                TargetEntity = Entity.Null,
                TargetEntityWorldSquareRadius = defaultStopMoveWorldRadius,
                TargetPosition = selectedPos,
                NewMoveCommandSource = MoveCommandSource.PlayerCommand,
                MoveCommandSourceMap = moveCommandSourceMap.Value,
                SpeedArray = speedArray,
            }.ScheduleParallel(getSpeedsJobHandle);

            state.Dependency = speedArray.Dispose(setTargetJobHandle);

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
using Unity.Entities;
using Core;
using Components;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Transforms;
using Components.Unit.UnitSelection;
using Core.Unit.MyMoveCommand;
using Components.Unit.MyMoveCommand;
using Utilities.Jobs;
using Components.Misc.WorldMap;
using Utilities.Helpers;
using Components.GameEntity;
using Core.GameEntity;
using Unity.Collections;
using Components.Unit.Reaction;
using Components.Misc.WorldMap.PathFinding;
using Components.Unit;

namespace Systems.Simulation.GameEntity
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DragSelectionSystem))]
    [BurstCompile]
    public partial struct SelectInteractableEntitySystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InteractableEntityTag>();
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
            state.RequireForUpdate<CellRadius>();
            state.RequireForUpdate<GameEntitySizeMap>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveCommandSourceMap = SystemAPI.GetSingleton<MoveCommandSourceMap>();
            var gameEntitySizeMap = SystemAPI.GetSingleton<GameEntitySizeMap>().Value;
            var cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;
            var unitReactionConfigsMap = SystemAPI.GetSingleton<UnitReactionConfigsMap>().Value;

            // Checking Hit Data.
            bool canGetInteractable = this.TryGetInteractable(
                ref state
                , in gameEntitySizeMap
                , in cellRadius
                , out Entity entity
                , out float3 pos
                , out half worldSquareRadius);

            if (!canGetInteractable) return;

            var speedArray = new NativeArray<float>(this.entityQuery.CalculateEntityCount(), Allocator.TempJob);

            var getSpeedsJobHandle = new GetRunSpeedsJob()
            {
                UnitReactionConfigsMap = unitReactionConfigsMap,
                OutputArray = speedArray,
            }.ScheduleParallel(state.Dependency);

            var setTargetJobHandle = new SetSingleTargetJobMultipleSpeeds()
            {
                TargetEntity = entity,
                TargetEntityWorldSquareRadius = worldSquareRadius,
                TargetPosition = pos,
                NewMoveCommandSource = MoveCommandSource.PlayerCommand,
                MoveCommandSourceMap = moveCommandSourceMap.Value,
                SpeedArray = speedArray,
            }.ScheduleParallel(getSpeedsJobHandle);

            state.Dependency = speedArray.Dispose(setTargetJobHandle);

        }

        [BurstCompile]
        private bool TryGetInteractable(
            ref SystemState state
            , in NativeHashMap<Entity, GameEntitySize> gameEntitySizeMap
            , in half cellRadius
            , out Entity entity
            , out float3 pos
            , out half worldSquareRadius)
        {
            entity = Entity.Null;
            pos = float3.zero;
            worldSquareRadius = half.zero;

            var selectionHits = SystemAPI.GetSingletonBuffer<SelectionHitElement>();
            if (selectionHits.IsEmpty) return false;
            int length = selectionHits.Length;

            for (int i = 0; i < length; i++)
            {
                var hit = selectionHits[i];
                if (hit.SelectionType != SelectionType.InteractableEntity) continue;

                entity = hit.HitEntity;
                pos = SystemAPI.GetComponent<LocalTransform>(entity).Position;

                var primaryPrefabEntityHolder = SystemAPI.GetComponent<PrimaryPrefabEntityHolder>(entity);
                gameEntitySizeMap.TryGetValue(primaryPrefabEntityHolder, out var gameEntitySize);

                int entityGridSquareSize = gameEntitySize.GridSquareSize;// TODO: We can look for EntityGridSquareSize in some map.
                float entityWorldSquareSize =
                    WorldMapHelper.GridLengthToWorldLength(in cellRadius, entityGridSquareSize);

                worldSquareRadius = new(entityWorldSquareSize / 2);

                selectionHits.RemoveAt(i);
                return true;
            }

            return false;
        }

    }

}
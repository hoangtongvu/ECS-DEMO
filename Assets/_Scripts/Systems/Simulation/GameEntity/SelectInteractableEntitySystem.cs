using Components;
using Components.GameEntity;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.PathFinding;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.MyMoveCommand;
using Components.Unit.Reaction;
using Components.Unit.UnitSelection;
using Core;
using Core.GameEntity;
using Core.Unit.MyMoveCommand;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utilities.Helpers;
using Utilities.Jobs;

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
            var moveCommandPrioritiesMap = SystemAPI.GetSingleton<MoveCommandPrioritiesMap>();
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
                MoveCommandPrioritiesMap = moveCommandPrioritiesMap,
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
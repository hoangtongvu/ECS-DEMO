using Components;
using Components.GameEntity;
using Components.GameEntity.Interaction;
using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.PathFinding;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.Reaction;
using Core;
using Core.GameEntity;
using Core.GameEntity.Movement.MoveCommand;
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
        private EntityQuery setCanOverrideMoveCommandTagJobQuery;
        private EntityQuery setTargetJobQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<InteractableEntityTag>();
            state.RequireForUpdate<SelectionHitElement>();
            state.RequireForUpdate<MoveCommandPrioritiesMap>();

            this.setCanOverrideMoveCommandTagJobQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder>()
                .WithAll<
                    MoveCommandElement
                    , InteractingEntity
                    , InteractionTypeICD
                    , ArmedStateHolder>()
                .WithAll<
                    CanSetTargetJobScheduleTag>()
                .WithPresent<
                    CanOverrideMoveCommandTag>()
                .Build();

            this.setTargetJobQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder>()
                .WithAll<
                    LocalTransform
                    , AbsoluteDistanceXZToTarget
                    , MoveSpeedLinear
                    , TargetEntity
                    , TargetEntityWorldSquareRadius
                    , MoveCommandElement
                    , InteractableDistanceRange>()
                .WithAll<
                    CanSetTargetJobScheduleTag
                    , CanOverrideMoveCommandTag>()
                .WithPresent<
                    CanFindPathTag>()
                .Build();

            state.RequireForUpdate(this.setCanOverrideMoveCommandTagJobQuery);
            state.RequireForUpdate(this.setTargetJobQuery);
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

            state.Dependency = new Set_CanSetTargetJobScheduleTag_OnUnitSelected()
                .ScheduleParallel(state.Dependency);

            state.Dependency = new SetCanOverrideMoveCommandTagJob
            {
                MoveCommandPrioritiesMap = moveCommandPrioritiesMap,
                NewMoveCommandSource = MoveCommandSource.PlayerCommand,
            }.ScheduleParallel(this.setCanOverrideMoveCommandTagJobQuery, state.Dependency);

            state.Dependency = new SetSpeedsAsRunSpeedsJob()
            {
                UnitReactionConfigsMap = unitReactionConfigsMap,
            }.ScheduleParallel(this.setTargetJobQuery, state.Dependency);

            state.Dependency = new SetSingleTargetJob()
            {
                TargetEntity = entity,
                TargetEntityWorldSquareRadius = worldSquareRadius,
                TargetPosition = pos,
            }.ScheduleParallel(this.setTargetJobQuery, state.Dependency);

            state.Dependency = new CleanTagsJob().ScheduleParallel(state.Dependency);

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
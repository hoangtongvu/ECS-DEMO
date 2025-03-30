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
using Components.Misc.GlobalConfigs;
using Components.Misc.WorldMap;
using Utilities.Helpers;
using Components.GameEntity;

namespace Systems.Simulation.GameEntity
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
            state.RequireForUpdate<MoveCommandSourceMap>();

            EntityQuery query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitSelectedTag
                    , CanMoveEntityTag
                    , LocalTransform
                    , DistanceToTarget
                    , TargetEntity
                    , TargetPosition
                    , MoveCommandElement>()
                .Build();

            state.RequireForUpdate(query);
            state.RequireForUpdate<CellRadius>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveCommandSourceMap = SystemAPI.GetSingleton<MoveCommandSourceMap>();
            var gameGlobalConfigs = SystemAPI.GetSingleton<GameGlobalConfigsICD>();
            var cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;

            // Checking Hit Data.
            bool canGetInteractable = this.TryGetInteractable(ref state, in cellRadius, out Entity entity, out float3 pos, out half worldSquareRadius);
            if (!canGetInteractable) return;

            state.Dependency = new SetTargetJob()
            {
                TargetEntity = entity,
                TargetEntityWorldSquareRadius = worldSquareRadius,
                TargetPosition = pos,
                NewMoveCommandSource = MoveCommandSource.PlayerCommand,
                UnitMoveSpeed = gameGlobalConfigs.Value.UnitRunSpeed,
                MoveCommandSourceMap = moveCommandSourceMap.Value,
            }.ScheduleParallel(state.Dependency);

        }

        [BurstCompile]
        private bool TryGetInteractable(
            ref SystemState state
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

                int entityGridSquareSize = SystemAPI.GetComponent<EntityGridSquareSize>(entity).Value;
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
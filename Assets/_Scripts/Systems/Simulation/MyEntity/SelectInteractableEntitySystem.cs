using Unity.Entities;
using Core;
using Components;
using Unity.Mathematics;
using Unity.Burst;
using Components.MyEntity;
using Unity.Transforms;
using Components.Unit.UnitSelection;
using Core.Unit.MyMoveCommand;
using Components.Unit.MyMoveCommand;
using Utilities.Jobs;
using Components.Misc.GlobalConfigs;

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
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveCommandSourceMap = SystemAPI.GetSingleton<MoveCommandSourceMap>();
            var gameGlobalConfigs = SystemAPI.GetSingleton<GameGlobalConfigsICD>();

            // Checking Hit Data.
            if (!this.TryGetInteractable(out Entity entity, out float3 pos)) return;

            state.Dependency = new SetTargetJob()
            {
                TargetEntity = entity,
                TargetPosition = pos,
                NewMoveCommandSource = MoveCommandSource.PlayerCommand,
                UnitMoveSpeed = gameGlobalConfigs.Value.UnitRunSpeed,
                MoveCommandSourceMap = moveCommandSourceMap.Value,
            }.ScheduleParallel(state.Dependency);

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

    }

}
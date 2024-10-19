using Unity.Entities;
using Components.Unit;
using Core;
using Components;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Utilities.Helpers;
using Components.MyEntity;
using Unity.Transforms;
using Components.Unit.UnitSelection;
using Core.Unit.MyMoveCommand;
using Components.Unit.MyMoveCommand;

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

            // Checking Hit Data.
            if (!this.TryGetInteractable(out Entity entity, out float3 pos)) return;

            new SetTargetJob()
            {
                targetEntity = entity,
                targetPosition = pos,
                moveCommandSourceMap = moveCommandSourceMap.Value,
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
            [ReadOnly] public NativeHashMap<MoveCommandSourceId, byte> moveCommandSourceMap;

            void Execute(
                in UnitId unitId
                , EnabledRefRO<UnitSelectedTag> unitSelectedTag
                , EnabledRefRW<CanMoveEntityTag> canMoveEntityTag
                , in LocalTransform transform
                , ref DistanceToTarget distanceToTarget
                , ref TargetPosition targetPosition
                , EnabledRefRW<TargetPosChangedTag> targetPosChangedTag
                , ref TargetEntity targetInteractableEntity
                , ref MoveCommandElement moveCommandElement
                , ref InteractingEntity interactingEntity
                , ref InteractionTypeICD interactionTypeICD)
            {
                bool unitSelected = unitSelectedTag.ValueRO;
                if (!unitSelected) return;

                bool canOverrideCommand =
                    MoveCommandHelper.TryOverrideMoveCommand(
                        in this.moveCommandSourceMap
                        , unitId.UnitType
                        , ref moveCommandElement
                        , ref interactingEntity
                        , ref interactionTypeICD
                        , MoveCommandSource.PlayerCommand
                        , unitId.LocalIndex);

                if (!canOverrideCommand) return;

                canMoveEntityTag.ValueRW = true;
                targetInteractableEntity.Value = this.targetEntity;
                targetPosition.Value = this.targetPosition;
                targetPosChangedTag.ValueRW = true;

                moveCommandElement.Float3 = this.targetPosition;
                moveCommandElement.TargetEntity = this.targetEntity;

                // Init distance to target.
                distanceToTarget.CurrentDistance = MathHelper.GetDistance2(transform.Position, this.targetPosition);

            }
        }

    }
}
using Unity.Entities;
using Components.Unit;
using Core;
using Components;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;
using Components.Unit.UnitSelection;
using Utilities.Helpers;
using Core.Unit.MyMoveCommand;
using Components.Unit.MyMoveCommand;
using Components.Misc.GlobalConfigs;
using Components.MyEntity;

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
            state.RequireForUpdate<MoveCommandSourceMap>();

            EntityQuery query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitId
                    , UnitSelectedTag
                    , CanMoveEntityTag
                    , TargetPosition
                    , MoveCommandElement
                    , MoveSpeedLinear>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Checking Hit Data.
            if (!this.TryGetSelectedPosition(out float3 selectedPos)) return;

            var moveCommandSourceMap = SystemAPI.GetSingleton<MoveCommandSourceMap>();
            var gameGlobalConfigs = SystemAPI.GetSingleton<GameGlobalConfigsICD>();

            new SetTargetJob()
            {
                targetPosition = selectedPos,
                unitRunSpeed = gameGlobalConfigs.Value.UnitRunSpeed,
                moveCommandSourceMap = moveCommandSourceMap.Value,
            }.Run();

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
            [ReadOnly] public float unitRunSpeed;
            [ReadOnly] public NativeHashMap<MoveCommandSourceId, byte> moveCommandSourceMap;

            void Execute(
                in UnitId unitId
                , EnabledRefRO<UnitSelectedTag> unitSelectedTag
                , EnabledRefRW<CanMoveEntityTag> canMoveEntityTag
                , ref MoveSpeedLinear moveSpeedLinear
                , ref TargetEntity targetEntity
                , ref TargetPosition targetPosition
                , EnabledRefRW<TargetPosChangedTag> targetPosChangedTag
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

                moveCommandElement.TargetEntity = Entity.Null;
                moveCommandElement.Float3 = this.targetPosition;
                targetEntity.Value = Entity.Null;
                targetPosition.Value = this.targetPosition;
                targetPosChangedTag.ValueRW = true;
                moveSpeedLinear.Value = this.unitRunSpeed;

                canMoveEntityTag.ValueRW = true;

            }
        }
    }
}
using Components.GameEntity;
using Components.GameEntity.Interaction;
using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Components.Misc.WorldMap.PathFinding;
using Components.Tool;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.MyMoveCommand;
using Components.Unit.Reaction;
using Core.Unit.MyMoveCommand;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utilities;
using Utilities.Helpers.Misc;

namespace Systems.Simulation.Tool
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct ToolCallerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , CanBePickedTag
                    , ToolPickerEntity
                    , DerelictToolTag>()
                .Build();

            var query1 = SystemAPI.QueryBuilder()
                .WithAll<
                    JoblessUnitTag
                    , LocalTransform
                    , TargetEntity
                    , MoveCommandElement
                    , UnitProfileIdHolder>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate(query1);

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new ToolCallRadius
                {
                    Value = 15f,
                });

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveCommandPrioritiesMap = SystemAPI.GetSingleton<MoveCommandPrioritiesMap>();
            var toolCallRadius = SystemAPI.GetSingleton<ToolCallRadius>();
            var unitReactionConfigsMap = SystemAPI.GetSingleton<UnitReactionConfigsMap>().Value;
            const float targetEntityWorldSquareRadius = 0.5f; // TODO: Find another way to get this value.

            foreach (var (toolTransformRef, toolEntity) in
                SystemAPI.Query<
                    RefRO<LocalTransform>>()
                    .WithEntityAccess()
                    .WithAll<DerelictToolTag>())
            {
                foreach (var (
                    unitTransformRef
                    , targetEntityRef
                    , worldSquareRadiusRef
                    , moveCommandElementRef
                    , unitProfileIdHolderRef
                    , interactingEntityRef
                    , interactionTypeICDRef
                    , unitEntity) in
                    SystemAPI.Query<
                        RefRO<LocalTransform>
                        , RefRW<TargetEntity>
                        , RefRW<TargetEntityWorldSquareRadius>
                        , RefRW<MoveCommandElement>
                        , RefRO<UnitProfileIdHolder>
                        , RefRW<InteractingEntity>
                        , RefRW<InteractionTypeICD>>()
                        .WithAll<JoblessUnitTag>()
                        .WithEntityAccess())
                {
                    bool unitIsCalled = moveCommandElementRef.ValueRO.CommandSource == MoveCommandSource.ToolCall;
                    if (unitIsCalled) continue;

                    float distance = math.distance(toolTransformRef.ValueRO.Position, unitTransformRef.ValueRO.Position);
                    if (distance > toolCallRadius.Value) continue;

                    var armedStateHolderRef = SystemAPI.GetComponentRO<ArmedStateHolder>(unitEntity);

                    bool canOverrideMoveCommand =
                        MoveCommandPrioritiesHelper.TryOverrideMoveCommand(
                            moveCommandPrioritiesMap
                            , ref moveCommandElementRef.ValueRW
                            , ref interactingEntityRef.ValueRW
                            , ref interactionTypeICDRef.ValueRW
                            , armedStateHolderRef.ValueRO.Value
                            , MoveCommandSource.ToolCall);

                    if (!canOverrideMoveCommand) continue;

                    moveCommandElementRef.ValueRW.Float3 = toolTransformRef.ValueRO.Position;
                    moveCommandElementRef.ValueRW.TargetEntity = toolEntity;

                    targetEntityRef.ValueRW.Value = toolEntity;
                    worldSquareRadiusRef.ValueRW.Value = new(targetEntityWorldSquareRadius);

                    if (!unitReactionConfigsMap.TryGetValue(unitProfileIdHolderRef.ValueRO.Value, out var unitReactionConfigs))
                        throw new KeyNotFoundException($"{nameof(UnitReactionConfigsMap)} does not contains key: {unitProfileIdHolderRef.ValueRO.Value}");

                    SystemAPI.SetComponent(unitEntity, new MoveSpeedLinear
                    {
                        Value = unitReactionConfigs.UnitRunSpeed,
                    });
                    SystemAPI.SetComponentEnabled<CanFindPathTag>(unitEntity, true);

                    var absoluteDistanceXZToTargetRef = SystemAPI.GetComponentRW<AbsoluteDistanceXZToTarget>(unitEntity);

                    AbsoluteDistanceXZToTargetHelper.SetDistance(
                        ref absoluteDistanceXZToTargetRef.ValueRW
                        , in unitTransformRef.ValueRO.Position
                        , in toolTransformRef.ValueRO.Position);

                }

            }

        }

    }

}
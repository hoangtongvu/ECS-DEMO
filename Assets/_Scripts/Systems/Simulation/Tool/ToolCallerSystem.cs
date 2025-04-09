using Unity.Entities;
using Unity.Burst;
using Components.Tool;
using Components;
using Components.Unit;
using Unity.Transforms;
using Unity.Mathematics;
using Systems.Simulation.Unit;
using Utilities.Helpers;
using Utilities;
using Components.Unit.MyMoveCommand;
using Core.Unit.MyMoveCommand;
using Components.GameEntity;
using Components.Misc.WorldMap.PathFinding;
using Components.Misc.GlobalConfigs;
using Utilities.Helpers.Misc;

namespace Systems.Simulation.Tool
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(SetCurrentDisToTargetSystem))]
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
                    , TargetPosition
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
            var commandSourceMap = SystemAPI.GetSingleton<MoveCommandSourceMap>();
            var toolCallRadius = SystemAPI.GetSingleton<ToolCallRadius>();
            var gameGlobalConfigs = SystemAPI.GetSingleton<GameGlobalConfigsICD>();
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

                    bool canOverrideMoveCommand =
                        MoveCommandHelper.TryOverrideMoveCommand(
                            commandSourceMap.Value
                            , unitProfileIdHolderRef.ValueRO.Value.UnitType
                            , ref moveCommandElementRef.ValueRW
                            , ref interactingEntityRef.ValueRW
                            , ref interactionTypeICDRef.ValueRW
                            , MoveCommandSource.ToolCall
                            , unitProfileIdHolderRef.ValueRO.Value.VariantIndex);

                    if (!canOverrideMoveCommand) continue;

                    moveCommandElementRef.ValueRW.Float3 = toolTransformRef.ValueRO.Position;
                    moveCommandElementRef.ValueRW.TargetEntity = toolEntity;

                    targetEntityRef.ValueRW.Value = toolEntity;
                    worldSquareRadiusRef.ValueRW.Value = new(targetEntityWorldSquareRadius);

                    SystemAPI.SetComponent(unitEntity, new MoveSpeedLinear
                    {
                        Value = gameGlobalConfigs.Value.UnitRunSpeed,
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
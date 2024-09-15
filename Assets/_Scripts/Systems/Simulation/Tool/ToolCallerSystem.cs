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
using Components.MyEntity;

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
                    , UnitId>()
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

            foreach (var (toolTransformRef, toolEntity) in
                SystemAPI.Query<
                    RefRO<LocalTransform>>()
                    .WithEntityAccess()
                    .WithAll<DerelictToolTag>())
            {

                foreach (var (unitTransformRef, targetEntityRef, targetPosRef, moveCommandElementRef, unitIdRef, unitEntity) in
                    SystemAPI.Query<
                        RefRO<LocalTransform>
                        , RefRW<TargetEntity>
                        , RefRW<TargetPosition>
                        , RefRW<MoveCommandElement>
                        , RefRO<UnitId>>()
                        .WithAll<JoblessUnitTag>()
                        .WithEntityAccess())
                {
                    
                    float distance = math.distance(toolTransformRef.ValueRO.Position, unitTransformRef.ValueRO.Position);
                    if (distance > toolCallRadius.Value) continue;

                    bool canOverrideMoveCommand =
                        MoveCommandHelper.TryOverrideMoveCommand(
                            commandSourceMap.Value
                            , unitIdRef.ValueRO.UnitType
                            , ref moveCommandElementRef.ValueRW
                            , MoveCommandSource.ToolCall
                            , unitIdRef.ValueRO.LocalIndex);

                    if (!canOverrideMoveCommand) continue;

                    targetEntityRef.ValueRW.Value = toolEntity;
                    targetPosRef.ValueRW.Value = toolTransformRef.ValueRO.Position;
                    SystemAPI.SetComponentEnabled<TargetPosChangedTag>(unitEntity, true);
                    moveCommandElementRef.ValueRW.Float3 = toolTransformRef.ValueRO.Position;
                    moveCommandElementRef.ValueRW.TargetEntity = toolEntity;
                    SystemAPI.SetComponentEnabled<CanMoveEntityTag>(unitEntity, true);

                }

                

            }


        }

        

    }
}
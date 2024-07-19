using Unity.Entities;
using Unity.Burst;
using Components.Tool;
using Components;
using Components.Unit;
using Unity.Transforms;
using Unity.Mathematics;
using Systems.Simulation.Unit;
using Core.Unit;
using Utilities.Helpers;
using Utilities;

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
                    , CanBePicked
                    , PickedBy
                    , DerelictToolTag>()
                .Build();

            var query1 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitToolHolder
                    , LocalTransform
                    , TargetPosition
                    , MoveAffecterICD
                    , DistanceToTarget
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
            var moveAffecterMap = SystemAPI.GetSingleton<MoveAffecterMap>();
            var toolCallRadius = SystemAPI.GetSingleton<ToolCallRadius>();

            foreach (var (toolTransformRef, canBePickedRef, pickedByRef, toolEntity) in
                SystemAPI.Query<
                    RefRO<LocalTransform>
                    , RefRW<CanBePicked>
                    , RefRW<PickedBy>>()
                    .WithEntityAccess()
                    .WithAll<DerelictToolTag>())
            {

                foreach (var (unitToolHolderRef, unitTransformRef, targetPosRef, moveAffecterRef, distanceToTargetRef, unitIdRef, unitEntity) in
                    SystemAPI.Query<
                        RefRW<UnitToolHolder>
                        , RefRO<LocalTransform>
                        , RefRW<TargetPosition>
                        , RefRW<MoveAffecterICD>
                        , RefRW<DistanceToTarget>
                        , RefRO<UnitId>>()
                        .WithEntityAccess())
                {
                    if (unitToolHolderRef.ValueRO.Value != Entity.Null) continue;
                    // Even if we can set min distance then how we can make unit pick up tool upon stop.


                    // calculate distance.
                    // compare to range.
                    float distance = math.distance(toolTransformRef.ValueRO.Position, unitTransformRef.ValueRO.Position);
                    if (distance > toolCallRadius.Value) continue;

                    if (distance <= distanceToTargetRef.ValueRO.MinDistance)
                    {
                        // Set as can be picked up.
                        canBePickedRef.ValueRW.Value = true;
                        pickedByRef.ValueRW.Value = unitEntity;

                        break;
                    }

                    if (!MoveAffecterHelper.TryChangeMoveAffecter(
                        in moveAffecterMap.Value
                        , unitIdRef.ValueRO.UnitType
                        , ref moveAffecterRef.ValueRW
                        , MoveAffecter.Others
                        , unitIdRef.ValueRO.LocalIndex))
                    {
                        continue;
                    }


                    targetPosRef.ValueRW.Value = toolTransformRef.ValueRO.Position;
                    SystemAPI.SetComponentEnabled<MoveableState>(unitEntity, true);

                }

                

            }


        }

        

    }
}
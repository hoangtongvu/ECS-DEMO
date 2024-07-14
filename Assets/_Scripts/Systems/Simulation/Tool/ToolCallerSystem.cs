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
                    ToolHolderElement
                    , LocalTransform
                    , ToolCallerRadius
                    , ToolPickRadius>()
                .Build();

            var query1 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitToolHolder
                    , LocalTransform
                    , TargetPosition
                    , MoveAffecterICD
                    , DistanceToTarget>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate(query1);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveAffecterMap = SystemAPI.GetSingleton<MoveAffecterMap>();

            foreach (var (toolsHolder, toolHolderTransformRef, toolCallerRadiusRef, toolPickRadiusRef) in
                SystemAPI.Query<
                    DynamicBuffer<ToolHolderElement>
                    , RefRO<LocalTransform>
                    , RefRO<ToolCallerRadius>
                    , RefRO<ToolPickRadius>>())
            {
                if (toolsHolder.IsEmpty) continue;
                int toolsHolderLength = toolsHolder.Length;

                for (int i = 0; i < toolsHolderLength; i++)
                {
                    ref var toolHolder = ref toolsHolder.ElementAt(i);

                    // May be adding a Jobless Tag to Unit more efficiently.

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
                        float distance = math.distance(toolHolderTransformRef.ValueRO.Position, unitTransformRef.ValueRO.Position);
                        if (distance > toolCallerRadiusRef.ValueRO.Value) continue;

                        if (distance <= toolPickRadiusRef.ValueRO.Value)
                        {
                            // Pick the tool
                            unitToolHolderRef.ValueRW.Value = toolHolder.Value;
                            toolsHolder.RemoveAt(i);
                            continue;
                        }

                        if (!MoveAffecterHelper.TryChangeMoveAffecter(
                            moveAffecterMap.Value
                            , unitIdRef.ValueRO.UnitType
                            , ref moveAffecterRef.ValueRW
                            , MoveAffecter.Others
                            , unitIdRef.ValueRO.LocalIndex))
                        {
                            continue;
                        }


                        targetPosRef.ValueRW.Value = toolHolderTransformRef.ValueRO.Position;
                        distanceToTargetRef.ValueRW.MinDistance = toolPickRadiusRef.ValueRO.Value;
                        SystemAPI.SetComponentEnabled<MoveableState>(unitEntity, true);

                    }

                    break; // process tool one by one.

                }

            }


        }

        

    }
}
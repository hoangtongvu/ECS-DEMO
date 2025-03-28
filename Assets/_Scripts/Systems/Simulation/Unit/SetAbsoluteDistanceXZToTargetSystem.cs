using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Components.Unit.MyMoveCommand;

namespace Systems.Simulation.Unit
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ChangeTargetPosWaypointSystem))]
    [BurstCompile]
    public partial struct SetAbsoluteDistanceXZToTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , LocalTransform
                    , AbsoluteDistanceXZToTarget
                    , MoveCommandElement>()
                .Build();

            state.RequireForUpdate(entityQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new SetAbsoluteDistanceXZ().ScheduleParallel();
        }

        [WithAll(typeof(CanMoveEntityTag))]
        [BurstCompile]
        private partial struct SetAbsoluteDistanceXZ : IJobEntity
        {
            [BurstCompile]
            private void Execute(
                in LocalTransform transform
                , ref AbsoluteDistanceXZToTarget absoluteDistanceXZToTarget
                , in MoveCommandElement moveCommandElement)
            {
                var tempDelta = moveCommandElement.Float3 - transform.Position;
                absoluteDistanceXZToTarget.X = math.abs(tempDelta.x);
                absoluteDistanceXZToTarget.Z = math.abs(tempDelta.z);

            }

        }

    }

}
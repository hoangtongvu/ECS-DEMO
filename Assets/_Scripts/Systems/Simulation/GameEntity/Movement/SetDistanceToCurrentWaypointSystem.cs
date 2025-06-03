using Components.GameEntity.Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Utilities.Helpers;

namespace Systems.Simulation.GameEntity.Movement
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ChangeCurrentWaypointSystem))]
    [BurstCompile]
    public partial struct SetDistanceToCurrentWaypointSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , LocalTransform
                    , DistanceToCurrentWaypoint
                    , CurrentWorldWaypoint>()
                .Build();

            state.RequireForUpdate(entityQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new SetDistanceJob().ScheduleParallel();
        }

        [WithAll(typeof(CanMoveEntityTag))]
        [BurstCompile]
        private partial struct SetDistanceJob : IJobEntity
        {
            [BurstCompile]
            private void Execute(
                in LocalTransform transform
                , ref DistanceToCurrentWaypoint distanceToCurrentWaypoint
                , in CurrentWorldWaypoint currentWorldWaypoint)
            {
                distanceToCurrentWaypoint.Value = MathHelper.GetDistance2(in transform.Position, in currentWorldWaypoint.Value);
            }

        }

    }

}
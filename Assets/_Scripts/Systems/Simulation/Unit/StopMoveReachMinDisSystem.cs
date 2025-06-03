using Components.GameEntity.Movement;
using Components.Misc;
using Components.Misc.WorldMap.PathFinding;
using Components.Unit.MyMoveCommand;
using Core.Unit.MyMoveCommand;
using Systems.Simulation.GameEntity.Movement;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Systems.Simulation.Unit
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SetDistanceToCurrentWaypointSystem))]
    [BurstCompile]
    public partial struct StopMoveReachMinDisSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , DistanceToCurrentWaypoint
                    , MoveCommandElement
                    , PhysicsVelocity>()
                .Build();

            state.RequireForUpdate(entityQuery);
            state.RequireForUpdate<DefaultStopMoveWorldRadius>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var defaultStopMoveWorldRadius = SystemAPI.GetSingleton<DefaultStopMoveWorldRadius>().Value;

            new StopMoveJob()
            {
                StopMoveWorldRadius = defaultStopMoveWorldRadius,
            }.ScheduleParallel();

        }

        [BurstCompile]
        private partial struct StopMoveJob : IJobEntity
        {
            [ReadOnly] public half StopMoveWorldRadius;

            [BurstCompile]
            void Execute(
                EnabledRefRW<CanMoveEntityTag> canMoveEntityTag
                , ref MoveCommandElement moveCommandElement
                , in DistanceToCurrentWaypoint distanceToCurrentWaypoint
                , ref PhysicsVelocity physicsVelocity
                , DynamicBuffer<WaypointElement> waypoints)
            {
                if (!waypoints.IsEmpty) return;
                if (Hint.Likely(distanceToCurrentWaypoint.Value >= this.StopMoveWorldRadius)) return;
                // velocityRef.ValueRW.Linear = 0;
                canMoveEntityTag.ValueRW = false;
                moveCommandElement.CommandSource = MoveCommandSource.None;
                moveCommandElement.TargetEntity = Entity.Null;

            }

        }

    }

}
using Components;
using Components.Misc.WorldMap.PathFinding;
using Components.Unit.MyMoveCommand;
using Core.Unit.MyMoveCommand;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Entities;
using Unity.Physics;

namespace Systems.Simulation.Unit
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SetCurrentDisToTargetSystem))]
    [BurstCompile]
    public partial struct StopMoveReachMinDisSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , DistanceToTarget
                    , MoveCommandElement
                    , PhysicsVelocity>()
                .Build();

            state.RequireForUpdate(entityQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new StopMoveJob().ScheduleParallel();
        }

        [BurstCompile]
        private partial struct StopMoveJob : IJobEntity
        {
            [BurstCompile]
            void Execute(
                EnabledRefRW<CanMoveEntityTag> canMoveEntityTag
                , ref MoveCommandElement moveCommandElement
                , in DistanceToTarget distanceToTarget
                , ref PhysicsVelocity physicsVelocity
                , DynamicBuffer<WaypointElement> waypoints)
            {
                if (!waypoints.IsEmpty) return;
                if (Hint.Likely(distanceToTarget.CurrentDistance >= 0.1f)) return; // TODO: Find another way to get this min dis.
                // velocityRef.ValueRW.Linear = 0;
                canMoveEntityTag.ValueRW = false;
                moveCommandElement.CommandSource = MoveCommandSource.None;

            }

        }

    }

}
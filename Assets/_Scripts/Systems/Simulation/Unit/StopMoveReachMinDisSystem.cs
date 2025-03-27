using Components;
using Components.Misc;
using Components.Misc.WorldMap.PathFinding;
using Components.Unit.MyMoveCommand;
using Core.Unit.MyMoveCommand;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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
                , in DistanceToTarget distanceToTarget
                , ref PhysicsVelocity physicsVelocity
                , DynamicBuffer<WaypointElement> waypoints)
            {
                if (!waypoints.IsEmpty) return;
                if (Hint.Likely(distanceToTarget.CurrentDistance >= this.StopMoveWorldRadius)) return; // TODO: Find another way to get this min dis.
                // velocityRef.ValueRW.Linear = 0;
                canMoveEntityTag.ValueRW = false;
                moveCommandElement.CommandSource = MoveCommandSource.None;

            }

        }

    }

}
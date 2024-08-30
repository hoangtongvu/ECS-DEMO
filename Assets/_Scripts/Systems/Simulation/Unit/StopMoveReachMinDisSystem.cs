using Components;
using Components.Unit;
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
                    , MoveAffecterICD
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
            void Execute(
                EnabledRefRW<CanMoveEntityTag> canMoveEntityTag
                , in DistanceToTarget distanceToTarget
                , ref MoveAffecterICD moveAffecter
                , ref PhysicsVelocity physicsVelocity)
            {
                if (Hint.Likely(distanceToTarget.CurrentDistance >= distanceToTarget.MinDistance)) return;
                // velocityRef.ValueRW.Linear = 0;
                canMoveEntityTag.ValueRW = false;
                moveAffecter.Value = Core.Unit.MoveAffecter.None;
            }
        }

    }
}


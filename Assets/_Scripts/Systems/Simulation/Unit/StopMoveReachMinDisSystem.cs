using Components;
using Unity.Burst;
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
                    MoveableState
                    , DistanceToTarget
                    , PhysicsVelocity>()
                .Build();

            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Can't put this into Job directly cause SystemAPI used.
            foreach (var (disToTargetRef, velocityRef, moveStateRef) in
                SystemAPI.Query<
                    RefRO<DistanceToTarget>
                    , RefRW<PhysicsVelocity>
                    , RefRO<MoveableState>>())
            {
                if (disToTargetRef.ValueRO.CurrentDistance >= disToTargetRef.ValueRO.MinDistance) continue;
                // velocityRef.ValueRW.Linear = 0;
                SystemAPI.SetComponentEnabled<MoveableState>(moveStateRef.ValueRO.Entity, false);
            }
        }

    }
}


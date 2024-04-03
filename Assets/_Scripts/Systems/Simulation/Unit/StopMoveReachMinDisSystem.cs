using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;


namespace Systems.Simulation.Unit
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(SetCurrentDisToTargetSystem))]
    [BurstCompile]
    public partial struct StopMoveReachMinDisSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MoveableState>();
            state.RequireForUpdate<DistanceToTarget>();
            state.RequireForUpdate<PhysicsVelocity>();
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


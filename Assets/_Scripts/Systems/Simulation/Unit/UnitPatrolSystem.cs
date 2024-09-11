using Unity.Entities;
using Unity.Burst;
using Components;
using Components.Damage;
using Components.Unit;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct UnitPatrolSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var (unitIdleRef, canMoveEntityTag, entity) in
                SystemAPI.Query<
                    RefRW<UnitIdleICD>
                    , EnabledRefRW<CanMoveEntityTag>>()
                    .WithAll<IsAliveTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                    .WithEntityAccess())
            {
                // Count the timer for idle state
                // On Idle end -> Make Unit to walk to a random position around
                // No MoveCommand left -> auto change CanMoveEntityTag disabled -> Idle state start again


                if (!canMoveEntityTag.ValueRO)
                {
                    // Also counter = 0??
                    unitIdleRef.ValueRW.TimeCounterSecond += SystemAPI.Time.DeltaTime;
                }


                if (unitIdleRef.ValueRO.TimeCounterSecond >= unitIdleRef.ValueRO.TimeDurationSecond)
                {
                    unitIdleRef.ValueRW.TimeCounterSecond = 0;
                    SystemAPI.SetComponentEnabled<NeedsInitWalkTag>(entity, true);
                }

            }


        }




    }
}
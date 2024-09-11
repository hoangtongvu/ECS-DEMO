using Unity.Entities;
using Unity.Burst;
using Components;
using Components.Damage;
using Components.Unit;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct UnitIdleSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var unitIdleICDRef in
                SystemAPI.Query<
                    RefRW<UnitIdleICD>>()
                    .WithAll<CanMoveEntityTag>())
            {
                unitIdleICDRef.ValueRW.TimeCounterSecond = 0;
            }

            foreach (var unitReactionRef in
                SystemAPI.Query<
                    RefRW<UnitReactionICD>>()
                    .WithAll<IsAliveTag>()
                    .WithDisabled<CanMoveEntityTag>())
            {
                unitReactionRef.ValueRW.Value = Core.Unit.UnitReaction.Idle;
            }


        }




    }
}
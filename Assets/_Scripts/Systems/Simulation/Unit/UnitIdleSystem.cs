using Unity.Entities;
using Unity.Burst;
using Components;
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
            foreach (var timeCounterRef in
                SystemAPI.Query<
                    RefRW<UnitIdleTimeCounter>>()
                    .WithAny<
                        CanMoveEntityTag
                        , IsUnitWorkingTag>())
            {
                timeCounterRef.ValueRW.Value = 0;
            }

            //foreach (var unitReactionRef in
            //    SystemAPI.Query<>()
            //        .WithAll<IsAliveTag>()
            //        .WithDisabled<CanMoveEntityTag>())
            //{
            //}


        }




    }
}
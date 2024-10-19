using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Components.Unit.NearUnitDropItems;

namespace Systems.Simulation.Unit.NearUnitDropItems
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct TimersCountUpSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    NearbyUnitDropItemTimerElement>()
                .Build();

            state.RequireForUpdate(query0);
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new CountUpJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel();

        }


        [BurstCompile]
        private partial struct CountUpJob : IJobEntity
        {
            [ReadOnly]
            public float DeltaTime;

            [BurstCompile]
            void Execute(
                DynamicBuffer<NearbyUnitDropItemTimerElement> nearbyUnitDropItemTimerList)
            {
                int length = nearbyUnitDropItemTimerList.Length;

                for (int i = 0; i < length; i++)
                {
                    nearbyUnitDropItemTimerList.ElementAt(i).CounterSecond += this.DeltaTime;
                }
            }
        }
        

    }

}
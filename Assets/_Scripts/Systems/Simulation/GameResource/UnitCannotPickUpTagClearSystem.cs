using Unity.Entities;
using Unity.Burst;
using Components.GameResource;
using Unity.Collections;
using Utilities;

namespace Systems.Simulation.GameResource
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct UnitCannotPickUpTagClearSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new UnitCannotPickUpTimeLimit
                {
                    Value = 5f,
                });

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitCannotPickUpTag
                    , UnitCannotPickUpTimeCounter>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<UnitCannotPickUpTimeLimit>();

        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float timeLimitSecond = SystemAPI.GetSingleton<UnitCannotPickUpTimeLimit>().Value;

            new CountUpAndClearTagJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                timeLimitSecond = timeLimitSecond,
            }.ScheduleParallel();

        }


        [BurstCompile]
        private partial struct CountUpAndClearTagJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;
            [ReadOnly] public float timeLimitSecond;

            [BurstCompile]
            void Execute(
                EnabledRefRW<UnitCannotPickUpTag> unitCannotPickUpTag
                , ref UnitCannotPickUpTimeCounter timeCounterSecond)
            {
                timeCounterSecond.CounterSecond += this.DeltaTime;

                if (timeCounterSecond.CounterSecond < timeLimitSecond) return;

                timeCounterSecond.CounterSecond = 0;
                unitCannotPickUpTag.ValueRW = false;

            }

        }

    }

}
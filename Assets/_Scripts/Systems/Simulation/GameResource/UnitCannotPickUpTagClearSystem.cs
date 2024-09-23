using Unity.Entities;
using Unity.Burst;
using Components.GameResource;
using Unity.Collections;

namespace Systems.Simulation.GameResource
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct UnitCannotPickUpTagClearSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitCannotPickUpTag
                    , UnitCannotPickUpTimeCounter>()
                .Build();

            state.RequireForUpdate(query0);

        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new CountUpAndClearTagJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel();

        }


        [BurstCompile]
        private partial struct CountUpAndClearTagJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;
            private const float timeLimitSecond = 5f; //TODO: Make this global config variable

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
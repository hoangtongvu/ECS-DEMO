using Components.GameEntity.Damage;
using Components.Unit.Misc;
using Components.Unit.UnitFeeding;
using Core.Unit.UnitFeeding;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Utilities;
using Utilities.Extensions.GameEntity.Damage;

namespace Systems.Simulation.Unit.UnitFeeding
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct StarvingDmgDealSystem : ISystem
    {
        private EntityQuery starvingDmgTakenEventQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddComponent<StarvingDmgDealTimerSeconds>();

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    IsAlive>()
                .WithAll<
                    UnitTag
                    , CurrentHungerThreshold
                    , HungerBarValue
                    , HpChangeRecordElement
                    , StarvingDmgTakenEvent>()
                .Build();

            this.starvingDmgTakenEventQuery = SystemAPI.QueryBuilder()
                .WithAll<StarvingDmgTakenEvent>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<UnitFeedingConfigsHolder>();
            state.RequireForUpdate<StarvingDmgDealTimerSeconds>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.SetComponentEnabled<StarvingDmgTakenEvent>(this.starvingDmgTakenEventQuery, false);

            var configs = SystemAPI.GetSingleton<UnitFeedingConfigsHolder>().Value;
            var timerSecondsRef = SystemAPI.GetSingletonRW<StarvingDmgDealTimerSeconds>();

            float intervalSeconds = configs.HungerBarConfigs.StarvingThresholdConfigs.StarvingTakeDmgIntervalMinutes * 60;
            timerSecondsRef.ValueRW += SystemAPI.Time.DeltaTime;

            if (timerSecondsRef.ValueRO < intervalSeconds) return;

            timerSecondsRef.ValueRW = 0;

            new StarvingDmgDealJob
            {
                BaseDmgTakenAmount = configs.HungerBarConfigs.StarvingThresholdConfigs.BaseDmgTakenPerInterval,
                QuadraticCoefficient = configs.HungerBarConfigs.StarvingThresholdConfigs.QuadraticCoefficientDmgTakenPerInterval,
                StarvingThresholdUpperBound = configs.HungerBarConfigs.StarvingThresholdConfigs.ThresholdUpperBound,
                StarvingDmgTakenEventLookup = SystemAPI.GetComponentLookup<StarvingDmgTakenEvent>(),
            }.ScheduleParallel();
        }

        [WithAll(typeof(UnitTag))]
        [WithAll(typeof(IsAlive))]
        [BurstCompile]
        private partial struct StarvingDmgDealJob : IJobEntity
        {
            [ReadOnly] public uint BaseDmgTakenAmount;
            [ReadOnly] public float QuadraticCoefficient;
            [ReadOnly] public float StarvingThresholdUpperBound;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<StarvingDmgTakenEvent> StarvingDmgTakenEventLookup;

            [BurstCompile]
            void Execute(
                in CurrentHungerThreshold currentHungerThreshold
                , in HungerBarValue hungerBarValue
                , ref DynamicBuffer<HpChangeRecordElement> hpChangeRecords
                , Entity entity)
            {
                if (currentHungerThreshold != HungerThreshold.Starving) return;

                float x = 1 - hungerBarValue / this.StarvingThresholdUpperBound;
                float rawDmgTaken = this.QuadraticCoefficient * math.square(x) + this.BaseDmgTakenAmount;

                hpChangeRecords.AddDeductRecord((int)math.floor(rawDmgTaken));
                this.StarvingDmgTakenEventLookup.SetComponentEnabled(entity, true);
            }

        }

    }

}
using Components.GameEntity.Damage;
using Components.Unit.Misc;
using Components.Unit.UnitFeeding;
using Core.Unit.UnitFeeding;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities;
using Utilities.Extensions.GameEntity.Damage;

namespace Systems.Simulation.Unit.UnitFeeding
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct FullThresholdHpRegenSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddComponent<FullThresholdHpRegenTimerSeconds>();

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    IsAlive>()
                .WithAll<
                    UnitTag
                    , CurrentHungerThreshold
                    , HpChangeRecordElement>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<UnitFeedingConfigsHolder>();
            state.RequireForUpdate<FullThresholdHpRegenTimerSeconds>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var configs = SystemAPI.GetSingleton<UnitFeedingConfigsHolder>().Value;
            var timerSecondsRef = SystemAPI.GetSingletonRW<FullThresholdHpRegenTimerSeconds>();

            float intervalSeconds = configs.HungerBarConfigs.FullThresholdConfigs.HpRegenIntervalSeconds;
            timerSecondsRef.ValueRW += SystemAPI.Time.DeltaTime;

            if (timerSecondsRef.ValueRO < intervalSeconds) return;

            timerSecondsRef.ValueRW = 0;

            new HpRegenJob
            {
                RegenAmount = configs.HungerBarConfigs.FullThresholdConfigs.HpRegenValuePerInterval,
            }.ScheduleParallel();
        }

        [WithAll(typeof(UnitTag))]
        [WithAll(typeof(IsAlive))]
        [BurstCompile]
        private partial struct HpRegenJob : IJobEntity
        {
            [ReadOnly] public uint RegenAmount;

            [BurstCompile]
            void Execute(
                in CurrentHungerThreshold currentHungerThreshold
                , ref DynamicBuffer<HpChangeRecordElement> hpChangeRecords)
            {
                if (currentHungerThreshold != HungerThreshold.Full) return;

                hpChangeRecords.AddRecord((int)this.RegenAmount);
            }

        }

    }

}
using Components.GameEntity.Damage;
using Components.Unit.Misc;
using Components.Unit.UnitFeeding;
using Core.Unit.UnitFeeding;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Simulation.Unit.UnitFeeding
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct HungerBarAutoDrainSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    IsAlive>()
                .WithAll<
                    UnitTag
                    , HungerBarValue>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var configs = UnitFeedingConfigConstants.UnitFeedingConfigs;
            float deductAmount = configs.HungerBarConfigs.HungerDrainSpeed * SystemAPI.Time.DeltaTime;

            state.Dependency = new HungerBarDrainJob
            {
                DeductAmount = deductAmount,
            }.ScheduleParallel(state.Dependency);
        }

        [WithAll(typeof(UnitTag))]
        [WithAll(typeof(IsAlive))]
        [BurstCompile]
        private partial struct HungerBarDrainJob : IJobEntity
        {
            [ReadOnly] public float DeductAmount;

            [BurstCompile]
            void Execute(
                ref HungerBarValue hungerBarValue)
            {
                if (hungerBarValue <= 0) return;

                hungerBarValue -= this.DeductAmount;
            }

        }

    }

}
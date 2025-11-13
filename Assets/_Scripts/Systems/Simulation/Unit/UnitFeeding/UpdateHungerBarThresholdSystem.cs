using Components.GameEntity.Damage;
using Components.Unit.Misc;
using Components.Unit.UnitFeeding;
using Core.Unit.UnitFeeding;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Simulation.Unit.UnitFeeding
{
    [UpdateInGroup(typeof(HungerBarThresholdChangesHandleSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct UpdateHungerBarThresholdSystem : ISystem
    {
        private EntityQuery query0;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    IsAlive>()
                .WithAll<
                    UnitTag
                    , HungerBarValue
                    , CurrentHungerThreshold
                    , HungerThresholdChanged>()
                .Build();

            state.RequireForUpdate(this.query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.SetComponentEnabled<HungerThresholdChanged>(this.query0, false);
            var configs = UnitFeedingConfigConstants.UnitFeedingConfigs;

            state.Dependency = new UpdateThresholdJob
            {
                hungerBarConfigs = configs.HungerBarConfigs,
            }.ScheduleParallel(state.Dependency);
        }

        [WithAll(typeof(UnitTag))]
        [WithAll(typeof(IsAlive))]
        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct UpdateThresholdJob : IJobEntity
        {
            [ReadOnly] public HungerBarConfigs hungerBarConfigs;

            [BurstCompile]
            void Execute(
                in HungerBarValue hungerBarValue
                , ref CurrentHungerThreshold currentHungerThreshold
                , EnabledRefRW<HungerThresholdChanged> thresholdChangedTag)
            {
                if (hungerBarValue < this.hungerBarConfigs.StarvingThresholdConfigs.ThresholdUpperBound)
                {
                    currentHungerThreshold = HungerThreshold.Starving;
                    thresholdChangedTag.ValueRW = true;
                    return;
                }

                if (hungerBarValue >= this.hungerBarConfigs.StarvingThresholdConfigs.ThresholdUpperBound
                    && hungerBarValue < this.hungerBarConfigs.NormalThresholdConfigs.ThresholdUpperBound)
                {
                    currentHungerThreshold = HungerThreshold.Normal;
                    thresholdChangedTag.ValueRW = true;
                    return;
                }

                currentHungerThreshold = HungerThreshold.Full;
                thresholdChangedTag.ValueRW = true;
            }

        }

    }

}
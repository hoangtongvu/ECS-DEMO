using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Core.Harvest;
using Components;
using Components.MyEntity;
using Core.MyEntity;
using Components.Misc;

namespace Systems.Simulation.Harvest
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct HarvestSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    InteractionTypeICD
                    , InteractingEntity
                    , BaseDmg
                    , BaseWorkSpeed
                    , WorkTimeCounterSecond>()
                .Build();

            state.RequireForUpdate(query0);
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var (interactionTypeICDRef, interactingEntityRef, baseDmgRef, baseWorkSpeedRef, workTimeCounterSecondRef, canMoveEntityTag) in
            SystemAPI.Query<
                RefRO<InteractionTypeICD>
                , RefRO<InteractingEntity>
                , RefRO<BaseDmg>
                , RefRO<BaseWorkSpeed>
                , RefRW<WorkTimeCounterSecond>
                , EnabledRefRO<CanMoveEntityTag>>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (canMoveEntityTag.ValueRO)
                {
                    workTimeCounterSecondRef.ValueRW.Value = 0;
                    continue;
                }

                if (interactionTypeICDRef.ValueRO.Value != InteractionType.Harvest) continue;

                var harvestEntity = interactingEntityRef.ValueRO.Value;
                bool noHarvestTargetFound = harvestEntity == Entity.Null;
                if (noHarvestTargetFound) continue;

                workTimeCounterSecondRef.ValueRW.Value += baseWorkSpeedRef.ValueRO.Value * SystemAPI.Time.DeltaTime;
                if (workTimeCounterSecondRef.ValueRO.Value < 1f) continue;
                workTimeCounterSecondRef.ValueRW.Value = 0;

                this.DealDmgToHarvestee(ref state, in harvestEntity, baseDmgRef.ValueRO.Value);

            }

        }

        [BurstCompile]
        private void DealDmgToHarvestee(
            ref SystemState state
            , in Entity harvestEntity
            , uint dmgValue)
        {
            var harvesteeHealthMap = SystemAPI.GetSingleton<HarvesteeHealthMap>();

            var healthId = new HealthId
            {
                Index = harvestEntity.Index,
                Version = harvestEntity.Version,
            };


            if (!harvesteeHealthMap.Value.TryGetValue(healthId, out var healthValue))
            {
                UnityEngine.Debug.LogError($"HarvesteeHealthMap does not contain {healthId}");
                return;
            }

            bool harvesteeIsDead = healthValue == 0;
            if (harvesteeIsDead) return;

            harvesteeHealthMap.Value[healthId] = healthValue <= dmgValue ? 0 : healthValue - dmgValue;

            SystemAPI.SetComponentEnabled<HarvesteeHealthChangedTag>(harvestEntity, true);

            UnityEngine.Debug.Log($"CurrHp = {harvesteeHealthMap.Value[healthId]}");

        }


    }
}
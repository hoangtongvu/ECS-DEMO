using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Core.Harvest;
using Components;
using Components.MyEntity;
using Core.MyEntity;
using Components.Misc;
using Components.Tool;
using Components.Unit;
using Core.Tool;
using Unity.Mathematics;

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
                    , WorkTimeCounterSecond
                    , ToolTypeICD
                    , HarvesteeTypeHolder>()
                .Build();

            state.RequireForUpdate(query0);
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var workTimeCounterSecondRef in
                SystemAPI.Query<
                    RefRW<WorkTimeCounterSecond>>()
                    .WithAll<CanMoveEntityTag>())
            {
                workTimeCounterSecondRef.ValueRW.Value = 0;
            }

            var harvesteeHealthMap = SystemAPI.GetSingleton<HarvesteeHealthMap>();
            var dmgBonusMap = SystemAPI.GetSingleton<Tool2HarvesteeDmgBonusMap>();


            foreach (var (interactionTypeICDRef, interactingEntityRef, baseDmgRef, baseWorkSpeedRef, workTimeCounterSecondRef, toolTypeRef, harvesteeTypeRef) in
                SystemAPI.Query<
                    RefRO<InteractionTypeICD>
                    , RefRO<InteractingEntity>
                    , RefRO<BaseDmg>
                    , RefRO<BaseWorkSpeed>
                    , RefRW<WorkTimeCounterSecond>
                    , RefRO<ToolTypeICD>
                    , RefRO<HarvesteeTypeHolder>>()
                    .WithDisabled<CanMoveEntityTag>())
            {

                if (interactionTypeICDRef.ValueRO.Value != InteractionType.Harvest) continue;

                var harvestEntity = interactingEntityRef.ValueRO.Value;
                bool noHarvestTargetFound = harvestEntity == Entity.Null;
                if (noHarvestTargetFound) continue;

                workTimeCounterSecondRef.ValueRW.Value += baseWorkSpeedRef.ValueRO.Value * SystemAPI.Time.DeltaTime;
                if (workTimeCounterSecondRef.ValueRO.Value < 1f) continue;
                workTimeCounterSecondRef.ValueRW.Value = 0;

                this.DealDmgToHarvestee(
                    ref state
                    , ref harvesteeHealthMap
                    , in dmgBonusMap
                    , toolTypeRef.ValueRO.Value
                    , harvesteeTypeRef.ValueRO.Value
                    , in harvestEntity
                    , baseDmgRef.ValueRO.Value);

            }

        }

        [BurstCompile]
        private void DealDmgToHarvestee(
            ref SystemState state
            , ref HarvesteeHealthMap harvesteeHealthMap
            , in Tool2HarvesteeDmgBonusMap dmgBonusMap
            , ToolType toolType
            , HarvesteeType harvesteeType
            , in Entity harvestEntity
            , uint baseDmgValue)
        {
            float bonusValue = this.GetDmgBonusValue(in dmgBonusMap, toolType, harvesteeType);
            uint finalDmg = (uint) math.round(baseDmgValue * bonusValue); // this is round down.

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

            harvesteeHealthMap.Value[healthId] = healthValue <= finalDmg ? 0 : healthValue - finalDmg;

            SystemAPI.SetComponentEnabled<HarvesteeHealthChangedTag>(harvestEntity, true);

            UnityEngine.Debug.Log($"CurrHp = {harvesteeHealthMap.Value[healthId]}");

        }

        [BurstCompile]
        private float GetDmgBonusValue(
            in Tool2HarvesteeDmgBonusMap dmgBonusMap
            , ToolType toolType
            , HarvesteeType harvesteeType)
        {

            var bonusId = new ToolHarvesteePairId
            {
                ToolType = toolType,
                HarvesteeType = harvesteeType,
            };

            if (!dmgBonusMap.Value.TryGetValue(bonusId, out float bonusValue))
            {
                UnityEngine.Debug.LogError($"DmgBonusMap does not contain {bonusId}");
                return 0;
            }

            return bonusValue;
        }


    }
}
using Unity.Entities;
using Unity.Burst;
using Core.Harvest;
using Components.Misc;
using Components.Tool;
using Components.Unit;
using Core.Tool;
using Unity.Mathematics;
using Core.GameEntity;
using Components.GameEntity;
using Components.Harvest.HarvesteeHp;
using System.Collections.Generic;
using Components.Unit.Misc;
using Components.GameEntity.Movement;

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

            var currentHpMap = SystemAPI.GetSingleton<HarvesteeCurrentHpMap>();
            var dmgBonusMap = SystemAPI.GetSingleton<Tool2HarvesteeDmgBonusMap>();

            foreach (var (interactionTypeICDRef, interactingEntityRef, baseDmgRef, baseWorkSpeedRef, workTimeCounterSecondRef, toolTypeRef, harvesteeTypeRef, entity) in
                SystemAPI.Query<
                    RefRO<InteractionTypeICD>
                    , RefRO<InteractingEntity>
                    , RefRO<BaseDmg>
                    , RefRO<BaseWorkSpeed>
                    , RefRW<WorkTimeCounterSecond>
                    , RefRO<ToolTypeICD>
                    , RefRO<HarvesteeTypeHolder>>()
                    .WithDisabled<CanMoveEntityTag>()
                    .WithEntityAccess())
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
                    , ref currentHpMap
                    , in dmgBonusMap
                    , toolTypeRef.ValueRO.Value
                    , harvesteeTypeRef.ValueRO.Value
                    , in harvestEntity
                    , baseDmgRef.ValueRO.Value);

                SystemAPI.SetComponentEnabled<CanCheckInteractionRepeatTag>(entity, true);

            }

        }

        [BurstCompile]
        private void DealDmgToHarvestee(
            ref SystemState state
            , ref HarvesteeCurrentHpMap currentHpMap
            , in Tool2HarvesteeDmgBonusMap dmgBonusMap
            , ToolType toolType
            , HarvesteeType harvesteeType
            , in Entity harvesteeEntity
            , uint baseDmgValue)
        {
            float bonusValue = this.GetDmgBonusValue(in dmgBonusMap, toolType, harvesteeType);
            uint finalDmg = (uint) math.round(baseDmgValue * bonusValue); // this is round down.

            if (!currentHpMap.Value.TryGetValue(harvesteeEntity, out var currentHp))
                throw new KeyNotFoundException($"{nameof(HarvesteeCurrentHpMap)} does not contain key: {harvesteeEntity}");

            bool harvesteeIsDead = currentHp == 0;
            if (harvesteeIsDead) return;

            currentHpMap.Value[harvesteeEntity] = currentHp <= finalDmg ? 0 : currentHp - finalDmg;

            SystemAPI.SetComponentEnabled<HarvesteeHpChangedTag>(harvesteeEntity, true);

            UnityEngine.Debug.Log($"CurrHp = {currentHpMap.Value[harvesteeEntity]}");

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
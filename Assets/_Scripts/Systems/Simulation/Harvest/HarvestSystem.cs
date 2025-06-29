using Components.GameEntity.Damage;
using Components.GameEntity.Interaction;
using Components.GameEntity.Movement;
using Components.Misc;
using Components.Tool;
using Components.Tool.Misc;
using Components.Unit;
using Core.GameEntity;
using Core.Harvest;
using Core.Tool;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Utilities.Extensions.GameEntity.Damage;
using static Utilities.Helpers.Misc.InteractionHelper;

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
                    , ToolProfileIdHolder
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

            var dmgBonusMap = SystemAPI.GetSingleton<Tool2HarvesteeDmgBonusMap>();

            foreach (var (interactionTypeICDRef, interactingEntityRef, baseDmgRef, baseWorkSpeedRef, workTimeCounterSecondRef, toolProfileIdHolderRef, harvesteeTypeRef, entity) in
                SystemAPI.Query<
                    RefRW<InteractionTypeICD>
                    , RefRW<InteractingEntity>
                    , RefRO<BaseDmg>
                    , RefRO<BaseWorkSpeed>
                    , RefRW<WorkTimeCounterSecond>
                    , RefRO<ToolProfileIdHolder>
                    , RefRO<HarvesteeTypeHolder>>()
                    .WithDisabled<CanMoveEntityTag>()
                    .WithEntityAccess())
            {
                if (interactionTypeICDRef.ValueRO.Value != InteractionType.Harvest) continue;

                var harvestEntity = interactingEntityRef.ValueRO.Value;

                // NOTE: harvestEntity has Child Buffer (ICleanupBuffer) -> harvestEntity destruction will be delayed -> can't check using Exists().
                if (!SystemAPI.HasComponent<IsAliveTag>(harvestEntity)) continue;

                bool isHarvesteeAlive = SystemAPI.IsComponentEnabled<IsAliveTag>(harvestEntity);
                if (!isHarvesteeAlive)
                {
                    StopInteraction(ref interactingEntityRef.ValueRW, ref interactionTypeICDRef.ValueRW);
                    continue;
                }

                workTimeCounterSecondRef.ValueRW.Value += baseWorkSpeedRef.ValueRO.Value * SystemAPI.Time.DeltaTime;
                if (workTimeCounterSecondRef.ValueRO.Value < 1f) continue;
                workTimeCounterSecondRef.ValueRW.Value = 0;

                this.DealDmgToHarvestee(
                    ref state
                    , in dmgBonusMap
                    , toolProfileIdHolderRef.ValueRO.Value.ToolType
                    , harvesteeTypeRef.ValueRO.Value
                    , in harvestEntity
                    , baseDmgRef.ValueRO.Value);

                SystemAPI.SetComponentEnabled<CanCheckInteractionRepeatTag>(entity, true);

            }

        }

        [BurstCompile]
        private void DealDmgToHarvestee(
            ref SystemState state
            , in Tool2HarvesteeDmgBonusMap dmgBonusMap
            , ToolType toolType
            , HarvesteeType harvesteeType
            , in Entity harvesteeEntity
            , uint baseDmgValue)
        {
            var hpChangeRecords = SystemAPI.GetBuffer<HpChangeRecordElement>(harvesteeEntity);

            float bonusValue = this.GetDmgBonusValue(in dmgBonusMap, toolType, harvesteeType);
            int finalDmg = (int) math.round(baseDmgValue * bonusValue); // this is round down.

            hpChangeRecords.AddDeductRecord(finalDmg);

        }

        [BurstCompile]
        private float GetDmgBonusValue(
            in Tool2HarvesteeDmgBonusMap dmgBonusMap
            , ToolType toolType
            , HarvesteeType harvesteeType)
        {
            const float defaultDmgScale = 0.7f;

            var bonusId = new ToolHarvesteePairId
            {
                ToolType = toolType,
                HarvesteeType = harvesteeType,
            };

            if (!dmgBonusMap.Value.TryGetValue(bonusId, out float dmgBonus))
                dmgBonus = defaultDmgScale;

            return dmgBonus;
        }

    }

}
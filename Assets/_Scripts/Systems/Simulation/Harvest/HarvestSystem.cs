using Unity.Entities;
using Unity.Burst;
using Components.Harvest;
using Core.Harvest;
using Components;
using Components.MyEntity;
using Core.MyEntity;

namespace Systems.Simulation.Harvest
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct HarvestSystem : ISystem
    {
        private uint dmg;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    InteractionTypeICD
                    , InteractingEntity
                    , HarvestSpeed
                    , HarvestTimeCounterSecond>()
                .Build();

            state.RequireForUpdate(query0);
            
            this.dmg = 5;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var (interactionTypeICDRef, interactingEntityRef, harvestSpeedRef, harvestTimeCounterSecondRef, canMoveEntityTag) in
            SystemAPI.Query<
                RefRO<InteractionTypeICD>
                , RefRO<InteractingEntity>
                , RefRO<HarvestSpeed>
                , RefRW<HarvestTimeCounterSecond>
                , EnabledRefRO<CanMoveEntityTag>>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (canMoveEntityTag.ValueRO)
                {
                    harvestTimeCounterSecondRef.ValueRW.Value = 0;
                    continue;
                }

                if (interactionTypeICDRef.ValueRO.Value != InteractionType.Harvest) continue;

                var harvestEntity = interactingEntityRef.ValueRO.Value;
                bool noHarvestTargetFound = harvestEntity == Entity.Null;
                if (noHarvestTargetFound) continue;

                harvestTimeCounterSecondRef.ValueRW.Value += harvestSpeedRef.ValueRO.Value * SystemAPI.Time.DeltaTime;
                if (harvestTimeCounterSecondRef.ValueRO.Value < 1f) continue;
                harvestTimeCounterSecondRef.ValueRW.Value = 0;

                this.DealDmgToHarvestee(ref state, in harvestEntity);

            }

        }

        [BurstCompile]
        private void DealDmgToHarvestee(
            ref SystemState state
            , in Entity harvestEntity)
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


            harvesteeHealthMap.Value[healthId] = healthValue <= this.dmg ? 0 : healthValue - this.dmg;

            SystemAPI.SetComponentEnabled<HarvesteeHealthChangedTag>(harvestEntity, true);

            UnityEngine.Debug.Log($"CurrHp = {harvesteeHealthMap.Value[healthId]}");

        }


    }
}
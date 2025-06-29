using Components.GameEntity.Interaction;
using Components.GameEntity.Movement;
using Components.Misc;
using Components.Misc.WorldMap.WorldBuilding.Blueprint;
using Components.Unit.Misc;
using Core.GameEntity;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.UnitAndBuilding
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct ConstructBuildingSystem : ISystem
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
                    , IsBuilderUnitTag>()
                .Build();

            state.RequireForUpdate(query0);
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var workTimeCounterSecondRef in
                SystemAPI.Query<
                    RefRW<WorkTimeCounterSecond>>()
                    .WithAll<CanMoveEntityTag>()
                    .WithAll<IsBuilderUnitTag>())
            {
                workTimeCounterSecondRef.ValueRW.Value = 0;
            }

            foreach (var (interactionTypeICDRef, interactingEntityRef, baseDmgRef, baseWorkSpeedRef, workTimeCounterSecondRef, entity) in SystemAPI
                .Query<
                    RefRW<InteractionTypeICD>
                    , RefRW<InteractingEntity>
                    , RefRO<BaseDmg>
                    , RefRO<BaseWorkSpeed>
                    , RefRW<WorkTimeCounterSecond>>()
                .WithDisabled<CanMoveEntityTag>()
                .WithAll<IsBuilderUnitTag>()
                .WithEntityAccess())
            {
                if (interactionTypeICDRef.ValueRO.Value != InteractionType.Attack) continue;
                var interactingEntity = interactingEntityRef.ValueRO.Value;

                bool buildingInConstruction = SystemAPI.HasComponent<ConstructionRemaining>(interactingEntity);
                if (!buildingInConstruction)
                {
                    this.StopInteraction(ref interactingEntityRef.ValueRW, ref interactionTypeICDRef.ValueRW);
                    continue;
                }

                workTimeCounterSecondRef.ValueRW.Value += baseWorkSpeedRef.ValueRO.Value * SystemAPI.Time.DeltaTime;
                if (workTimeCounterSecondRef.ValueRO.Value < 1f) continue;
                workTimeCounterSecondRef.ValueRW.Value = 0;

                Construct(ref state, in interactingEntity, baseDmgRef.ValueRO.Value);

                SystemAPI.SetComponentEnabled<CanCheckInteractionRepeatTag>(entity, true);

            }

        }

        [BurstCompile]
        private void Construct(
            ref SystemState state
            , in Entity interactingEntity
            , uint dmgValue)
        {
            var constructionRemainingRef = SystemAPI.GetComponentRW<ConstructionRemaining>(interactingEntity);
            constructionRemainingRef.ValueRW.Value = constructionRemainingRef.ValueRO.Value < dmgValue
                ? 0
                : constructionRemainingRef.ValueRO.Value - dmgValue;
        }

        [BurstCompile]
        private void StopInteraction(
            ref InteractingEntity interactingEntity
            , ref InteractionTypeICD interactionTypeICD)
        {
            interactingEntity.Value = Entity.Null;
            interactionTypeICD.Value = InteractionType.None;
        }

    }

}
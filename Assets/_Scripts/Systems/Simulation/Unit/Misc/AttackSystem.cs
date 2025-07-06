using Components.GameEntity.Damage;
using Components.GameEntity.Interaction;
using Components.GameEntity.Movement;
using Components.Misc;
using Core.GameEntity;
using Unity.Burst;
using Unity.Entities;
using Utilities.Extensions.GameEntity.Damage;
using static Utilities.Helpers.Misc.InteractionHelper;

namespace Systems.Simulation.Unit.Misc
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct AttackSystem : ISystem
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
            foreach (var workTimeCounterSecondRef in
                SystemAPI.Query<
                    RefRW<WorkTimeCounterSecond>>()
                    .WithAll<CanMoveEntityTag>())
            {
                workTimeCounterSecondRef.ValueRW.Value = 0;
            }

            foreach (var (interactionTypeICDRef, interactingEntityRef, baseDmgRef, baseWorkSpeedRef, workTimeCounterSecondRef, entity) in
                SystemAPI.Query<
                    RefRW<InteractionTypeICD>
                    , RefRW<InteractingEntity>
                    , RefRO<BaseDmg>
                    , RefRO<BaseWorkSpeed>
                    , RefRW<WorkTimeCounterSecond>>()
                    .WithDisabled<CanMoveEntityTag>()
                    .WithEntityAccess())
            {
                if (interactionTypeICDRef.ValueRO.Value != InteractionType.Attack) continue;

                var interactingEntity = interactingEntityRef.ValueRO.Value;

                bool isValidAndAliveTarget =
                    SystemAPI.HasComponent<IsAliveTag>(interactingEntity) &&
                    SystemAPI.IsComponentEnabled<IsAliveTag>(interactingEntity);

                if (!isValidAndAliveTarget)
                {
                    StopInteraction(ref interactingEntityRef.ValueRW, ref interactionTypeICDRef.ValueRW);
                    continue;
                }

                workTimeCounterSecondRef.ValueRW.Value += baseWorkSpeedRef.ValueRO.Value * SystemAPI.Time.DeltaTime;
                if (workTimeCounterSecondRef.ValueRO.Value < 1f) continue;
                workTimeCounterSecondRef.ValueRW.Value = 0;

                this.DealDmg(ref state, interactingEntityRef.ValueRO.Value, (int)baseDmgRef.ValueRO.Value);

                SystemAPI.SetComponentEnabled<CanCheckInteractionRepeatTag>(entity, true);

            }

        }

        [BurstCompile]
        private void DealDmg(
            ref SystemState state
            , in Entity interactingEntity
            , int dmgValue)
        {
            var hpChangeRecords = SystemAPI.GetBuffer<HpChangeRecordElement>(interactingEntity);
            hpChangeRecords.AddDeductRecord(dmgValue);
        }

    }

}
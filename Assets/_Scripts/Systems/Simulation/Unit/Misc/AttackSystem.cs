using Unity.Entities;
using Unity.Burst;
using Components;
using Components.Misc;
using Core.GameEntity;
using Components.GameEntity;
using Components.Unit.Misc;

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
                    RefRO<InteractionTypeICD>
                    , RefRO<InteractingEntity>
                    , RefRO<BaseDmg>
                    , RefRO<BaseWorkSpeed>
                    , RefRW<WorkTimeCounterSecond>>()
                    .WithDisabled<CanMoveEntityTag>()
                    .WithEntityAccess())
            {
                if (interactionTypeICDRef.ValueRO.Value != InteractionType.Attack) continue;

                workTimeCounterSecondRef.ValueRW.Value += baseWorkSpeedRef.ValueRO.Value * SystemAPI.Time.DeltaTime;
                if (workTimeCounterSecondRef.ValueRO.Value < 1f) continue;
                workTimeCounterSecondRef.ValueRW.Value = 0;

                // Deal dmg to interacting entity.

                SystemAPI.SetComponentEnabled<CanCheckInteractionRepeatTag>(entity, true);

            }

        }

    }

}
using Components.GameEntity.Interaction.InteractionPhases;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.Interaction.InteractionPhases.InteractingPhaseHandle
{
    [UpdateInGroup(typeof(PhaseTagsHandleSystemGroup))]
    [BurstCompile]
    public partial struct PhaseTagsHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    InteractingPhase.StartedEvent
                    , InteractingPhase.EndedEvent
                    , InteractingPhase.CanUpdate
                    , InteractingPhase.Updating
                    , InteractingPhase.CanCancel
                    , InteractingPhase.CanceledEvent>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new PhaseTagsHandleJob().ScheduleParallel();
        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct PhaseTagsHandleJob : IJobEntity
        {
            void Execute(
                EnabledRefRW<InteractingPhase.StartedEvent> startedEvent
                , EnabledRefRW<InteractingPhase.EndedEvent> endedEvent
                , EnabledRefRO<InteractingPhase.CanUpdate> canUpdateTag
                , EnabledRefRW<InteractingPhase.Updating> updatingTag
                , EnabledRefRO<InteractingPhase.CanCancel> canCancelTag
                , EnabledRefRW<InteractingPhase.CanceledEvent> canceledEvent)
            {
                startedEvent.ValueRW = false;
                endedEvent.ValueRW = false;
                canceledEvent.ValueRW = false;

                if (canCancelTag.ValueRO && updatingTag.ValueRO)
                {
                    //OnReactionCanceled();
                    canceledEvent.ValueRW = true;
                    updatingTag.ValueRW = false;
                    return;
                }

                if (canUpdateTag.ValueRO)
                {
                    if (!startedEvent.ValueRO && !updatingTag.ValueRO)
                    {
                        //OnReactionStart();
                        startedEvent.ValueRW = true;
                        updatingTag.ValueRW = true;
                    }

                    //OnReactionUpdate();
                }
                else
                {
                    if (updatingTag.ValueRO && !endedEvent.ValueRO)
                    {
                        //OnReactionEnd();
                        endedEvent.ValueRW = true;
                        updatingTag.ValueRW = false;
                    }
                }
            }
        }

    }

}
using Unity.Entities;
using Unity.Burst;
using Systems.Simulation.GameEntity.Interaction.Common;
using Components.GameEntity.Interaction.InteractionPhases;
using Components.GameResource.ItemPicking;

namespace Systems.Simulation.GameResource.ItemPicking
{
    [UpdateInGroup(typeof(CanUpdateHandleSystemGroup))]
    [BurstCompile]
    public partial struct Interacting_CanUpdate_HandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ItemCanBePickedUpIndex
                    , InteractingPhase.CanUpdate>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new TagHandleJob().ScheduleParallel();
        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct TagHandleJob : IJobEntity
        {
            void Execute(
                in DynamicBuffer<ItemCanBePickedUpIndex> itemCanBePickedUpIndices
                , EnabledRefRW<InteractingPhase.CanUpdate> canUpdateTag)
            {
                canUpdateTag.ValueRW = itemCanBePickedUpIndices.Length > 0;
            }
        }

    }

}
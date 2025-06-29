using Components.GameEntity.InteractableActions;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(ActionUIsHandleSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct ActionsContainerUIShownTagHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CanShowActionsContainerUITag
                    , ActionsContainerUIShownTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (canShowUITag, uiShownTag) in SystemAPI
                .Query<
                    EnabledRefRO<CanShowActionsContainerUITag>
                    , EnabledRefRW<ActionsContainerUIShownTag>>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                uiShownTag.ValueRW = canShowUITag.ValueRO;
            }

        }

    }

}
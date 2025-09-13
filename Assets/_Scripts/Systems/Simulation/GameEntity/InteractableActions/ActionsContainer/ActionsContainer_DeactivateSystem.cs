using Components.GameEntity.InteractableActions;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.InteractableActions.ActionsContainer
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ActionsContainer_DeactivateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ActionsContainerUI_CD.Holder
                    , ActionsContainerUI_CD.CanShow
                    , ActionsContainerUI_CD.IsActive>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            foreach (var (uiHolderRef, entity) in SystemAPI
                .Query<
                    RefRO<ActionsContainerUI_CD.Holder>>()
                .WithDisabled<
                    ActionsContainerUI_CD.CanShow>()
                .WithAll<
                    ActionsContainerUI_CD.IsActive>()
                .WithEntityAccess())
            {
                var uiCtrl = uiHolderRef.ValueRO.Value.Value;

                uiCtrl.TriggerHiding();
                SystemAPI.SetComponentEnabled<ActionsContainerUI_CD.IsActive>(entity, false);
            }

        }

    }

}
using Components.GameEntity.InteractableActions;
using Core.UI;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.InteractableActions.ActionsContainer
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ActionsContainer_HideSystem : SystemBase
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
            foreach (var uiHolderRef in SystemAPI
                .Query<
                    RefRW<ActionsContainerUI_CD.Holder>>()
                .WithDisabled<
                    ActionsContainerUI_CD.CanShow>()
                .WithDisabled<
                    ActionsContainerUI_CD.IsActive>())
            {
                var uiCtrl = uiHolderRef.ValueRO.Value.Value;
                if (uiCtrl == null) continue;

                if (uiCtrl.State != UIState.Hidden) continue;
                uiHolderRef.ValueRW.Value = null;
            }

        }

    }

}
using Components.GameEntity.InteractableActions;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(ActionsContainerUpdateSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(ActionsContainerCanUpdateTagClearSystem))]
    public partial class SetFirstActionWhenActionsFilledSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ActionsContainerUI_CD.Holder
                    , ActionsContainerUI_CD.CanUpdate>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            if (!this.CanActionsContainerUpdate()) return;

            var actionsContainerUICtrl = SystemAPI.GetSingleton<ActionsContainerUI_CD.Holder>().Value.Value;
            var actionPanelsHolder = actionsContainerUICtrl.ActionPanelsHolder;

            actionsContainerUICtrl.ChosenActionPanelCtrl = actionPanelsHolder.Value.Count == 0
                ? null
                : actionPanelsHolder.Value[0];
        }

        private bool CanActionsContainerUpdate()
        {
            foreach (var canUpdateTag in SystemAPI
                .Query<EnabledRefRO<ActionsContainerUI_CD.CanUpdate>>())
            {
                return true;
            }

            return false;
        }

    }

}
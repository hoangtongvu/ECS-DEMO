using Components.GameEntity.InteractableActions;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(ActionsContainerUpdateSystemGroup), OrderFirst = true)]
    public partial class ActionPanelsHolderClearSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<ActionsContainerUI_CD.Holder>();
        }

        protected override void OnUpdate()
        {
            if (!this.CanActionsContainerUpdate()) return;

            var actionsContainerUICtrl = SystemAPI.GetSingleton<ActionsContainerUI_CD.Holder>().Value.Value;

            foreach (var actionPanel in actionsContainerUICtrl.ActionPanelsHolder.Value)
            {
                actionPanel.ReturnSelfToPool();
            }

            actionsContainerUICtrl.ActionPanelsHolder.Value.Clear();
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
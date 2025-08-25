using Components.GameEntity.InteractableActions;
using Components.UI.Pooling;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(ActionUIsHandleSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(ShowActionsContainerUISystem))]
    public partial class DespawnUIAfterActionTriggeredSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ActionsContainerUIHolder
                    , ActionsContainerUIShownTag
                    , NewlyActionTriggeredTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<UIPoolMapInitializedTag>();
        }

        protected override void OnUpdate()
        {
            foreach (var (actionsContainerUIHolderRef, uiShownTag) in SystemAPI
                .Query<
                    RefRW<ActionsContainerUIHolder>
                    , EnabledRefRW<ActionsContainerUIShownTag>>()
                .WithAll<NewlyActionTriggeredTag>())
            {
                this.DespawnActionsContainerAndAllActionPanels(
                    ref actionsContainerUIHolderRef.ValueRW);

                uiShownTag.ValueRW = false;

            }

        }

        private void DespawnActionsContainerAndAllActionPanels(
            ref ActionsContainerUIHolder actionsContainerUIHolder)
        {
            var actionsContainerUICtrl = actionsContainerUIHolder.Value.Value;
            actionsContainerUICtrl.ReturnSelfToPool();
            actionsContainerUIHolder.Value = null;
        }

    }

}
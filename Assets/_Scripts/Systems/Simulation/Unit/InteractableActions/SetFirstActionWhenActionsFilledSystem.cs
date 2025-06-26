using Components.Unit.InteractableActions;
using Unity.Entities;

namespace Systems.Simulation.Unit.InteractableActions
{
    [UpdateInGroup(typeof(ActionUIsHandleSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(ActionsContainerUIShownTagHandleSystem))]
    public partial class SetFirstActionWhenActionsFilledSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ActionsContainerUIHolder
                    , CanShowActionsContainerUITag
                    , ActionsContainerUIShownTag>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            foreach (var (actionsContainerUIHolderRef, canShowUITag, uiShownTag) in SystemAPI
                .Query<
                    RefRO<ActionsContainerUIHolder>
                    , EnabledRefRO<CanShowActionsContainerUITag>
                    , EnabledRefRO<ActionsContainerUIShownTag>>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!canShowUITag.ValueRO) continue;
                if (uiShownTag.ValueRO) continue;

                var actionsContainerUICtrl = actionsContainerUIHolderRef.ValueRO.Value.Value;
                var actionPanelsHolder = actionsContainerUICtrl.ActionPanelsHolder;

                actionsContainerUICtrl.ChosenActionPanelCtrl = actionPanelsHolder.Value.Count == 0
                    ? null
                    : actionPanelsHolder.Value[0];

            }

        }

    }

}
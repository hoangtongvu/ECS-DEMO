using Components.GameEntity.InteractableActions;
using Components.UI.Pooling;
using Core.UI.Identification;
using Core.UI.InteractableActionsPanel;
using Core.UI.Pooling;
using Core.Utilities.Extensions;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(ActionUIsHandleSystemGroup), OrderFirst = true)]
    public partial class ShowActionsContainerUISystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , ActionsContainerUIHolder
                    , CanShowActionsContainerUITag
                    , ActionsContainerUIShownTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<ActionsContainerUIOffsetY>();
            this.RequireForUpdate<UIPoolMapInitializedTag>();
        }

        protected override void OnUpdate()
        {
            half offsetY = SystemAPI.GetSingleton<ActionsContainerUIOffsetY>().Value;

            foreach (var (unitTransform, actionsContainerUIHolderRef, canShowUITag, uiShownTag) in SystemAPI
                .Query<
                    RefRO<LocalTransform>
                    , RefRW<ActionsContainerUIHolder>
                    , EnabledRefRO<CanShowActionsContainerUITag>
                    , EnabledRefRO<ActionsContainerUIShownTag>>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!canShowUITag.ValueRO)
                {
                    if (uiShownTag.ValueRO)
                    {
                        this.DespawnActionsContainerAndAllActionPanels(
                            ref actionsContainerUIHolderRef.ValueRW);
                    }

                    continue;
                }

                if (uiShownTag.ValueRO) continue;

                this.SpawnActionsContainerUI(
                    unitTransform.ValueRO.Position.Add(y: offsetY)
                    , ref actionsContainerUIHolderRef.ValueRW);

            }

        }

        private void SpawnActionsContainerUI(
            in float3 spawnPos
            , ref ActionsContainerUIHolder actionsContainerUIHolder)
        {
            var baseUICtrl = UICtrlPoolMap.Instance.Rent(UIType.ActionsContainerUI);
            baseUICtrl.transform.position = spawnPos;

            baseUICtrl.gameObject.SetActive(true);
            actionsContainerUIHolder.Value = (ActionsContainerUICtrl)baseUICtrl;
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
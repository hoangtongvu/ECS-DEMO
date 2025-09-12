using Components.GameEntity.InteractableActions;
using Core.UI.Identification;
using Core.UI.InteractableActionsPanel;
using Core.UI.Pooling;
using Core.Utilities.Extensions;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation.GameEntity.InteractableActions.ActionsContainer
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ActionsContainer_ShowSystem : SystemBase
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
                    RefRW<ActionsContainerUI_CD.Holder>>()
                .WithAll<
                    ActionsContainerUI_CD.CanShow>()
                .WithDisabled<
                    ActionsContainerUI_CD.IsActive>()
                .WithEntityAccess())
            {
                var uiCtrl = uiHolderRef.ValueRO.Value.Value;
                bool isUIHidden = uiCtrl == null;

                if (isUIHidden)
                {
                    uiCtrl = (ActionsContainerUICtrl)UICtrlPoolMap.Instance.Rent(UIType.ActionsContainerUI);

                    uiHolderRef.ValueRW.Value = uiCtrl;
                    uiCtrl.gameObject.SetActive(true);
                }
                else
                {
                    uiCtrl.Reuse();
                }

                SystemAPI.SetComponentEnabled<ActionsContainerUI_CD.IsActive>(entity, true);

                if (!SystemAPI.HasComponent<ActionsContainerUI_CD.CanUpdate>(entity)) continue;
                SystemAPI.SetComponentEnabled<ActionsContainerUI_CD.CanUpdate>(entity, true);
            }

        }

    }

}
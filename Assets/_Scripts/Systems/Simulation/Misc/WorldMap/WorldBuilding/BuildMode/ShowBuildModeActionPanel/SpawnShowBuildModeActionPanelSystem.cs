using Components.GameEntity.InteractableActions;
using Components.GameEntity.Misc;
using Components.Player;
using Components.UI.Pooling;
using Components.Unit.Misc;
using Core.UI.Identification;
using Core.UI.InteractableActionsPanel.ActionPanel.ShowBuildModeActionPanel;
using Core.UI.Pooling;
using Systems.Simulation.GameEntity.InteractableActions;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildMode.ShowBuildModeActionPanel
{
    [UpdateInGroup(typeof(ActionUIsHandleSystemGroup))]
    public partial class SpawnShowBuildModeActionPanelSystem : SystemBase
    {
        private EntityQuery playerQuery;

        protected override void OnCreate()
        {
            this.playerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , FactionIndex>()
                .Build();

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitTag
                    , ActionsContainerUIHolder
                    , CanShowActionsContainerUITag
                    , ActionsContainerUIShownTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate(this.playerQuery);
            this.RequireForUpdate<UIPoolMapInitializedTag>();
        }

        protected override void OnUpdate()
        {
            byte playerFactionIndex = this.playerQuery.GetSingleton<FactionIndex>().Value;

            foreach (var (factionIndexRef, actionsContainerUIHolderRef, canShowUITag, uiShownTag, entity) in SystemAPI
                .Query<
                    RefRO<FactionIndex>
                    , RefRO<ActionsContainerUIHolder>
                    , EnabledRefRO<CanShowActionsContainerUITag>
                    , EnabledRefRO<ActionsContainerUIShownTag>>()
                .WithAll<UnitTag>()
                .WithEntityAccess()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!canShowUITag.ValueRO) continue;
                if (uiShownTag.ValueRO) continue;
                if (factionIndexRef.ValueRO.Value != playerFactionIndex) continue;

                var actionContainer = actionsContainerUIHolderRef.ValueRO.Value.Value;
                float3 spawnPos = actionContainer.transform.position;

                var actionPanelCtrl = (ShowBuildModeActionPanelCtrl)UICtrlPoolMap.Instance
                    .Rent(UIType.ActionPanel_ShowBuildMode);
                actionPanelCtrl.transform.position = spawnPos;

                actionPanelCtrl.Initialize(in entity, 0, actionContainer);
                actionPanelCtrl.gameObject.SetActive(true);
                actionContainer.ActionPanelsHolder.Add(actionPanelCtrl);

            }

        }

    }

}
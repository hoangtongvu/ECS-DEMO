using Components.ComponentMap;
using Components.GameEntity.InteractableActions;
using Core.UI.Identification;
using Core.UI.InteractableActionsPanel.ActionPanel.RecruitActionPanel;
using Core.Utilities.Helpers;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(ActionUIsHandleSystemGroup))]
    public partial class SpawnTempActionPanelSystem : SystemBase
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
            this.RequireForUpdate<UIPrefabAndPoolMap>();
            this.RequireForUpdate<SpawnedUIMap>();
        }

        protected override void OnUpdate()
        {
            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();

            foreach (var (actionsContainerUIHolderRef, canShowUITag, uiShownTag, entity) in SystemAPI
                .Query<
                    RefRO<ActionsContainerUIHolder>
                    , EnabledRefRO<CanShowActionsContainerUITag>
                    , EnabledRefRO<ActionsContainerUIShownTag>>()
                .WithEntityAccess()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!canShowUITag.ValueRO) continue;
                if (uiShownTag.ValueRO) continue;

                float3 spawnPos = actionsContainerUIHolderRef.ValueRO.Value.Value.transform.position;

                var actionPanelCtrl = (RecruitActionPanelCtrl)UISpawningHelper.Spawn(
                        uiPrefabAndPoolMap.Value
                        , spawnedUIMap.Value
                        , UIType.ActionPanel_Recruit
                        , spawnPos);

                actionPanelCtrl.Initialize(in entity, 0, actionsContainerUIHolderRef.ValueRO.Value);
                actionPanelCtrl.gameObject.SetActive(true);
                actionsContainerUIHolderRef.ValueRO.Value.Value.ActionPanelsHolder.Add(actionPanelCtrl);

            }

        }

    }

}
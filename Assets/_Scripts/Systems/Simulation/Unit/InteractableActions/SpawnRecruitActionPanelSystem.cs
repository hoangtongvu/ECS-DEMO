using Components.ComponentMap;
using Components.GameEntity;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.GameEntity.InteractableActions;
using Components.GameEntity.Misc;
using Components.GameResource;
using Core.GameResource;
using Core.UI.Identification;
using Core.UI.InteractableActionsPanel.ActionPanel.RecruitActionPanel;
using Core.UI.InteractableActionsPanel.ActionPanel.RecruitActionPanel.RecruitActionCostView;
using Core.Utilities.Helpers;
using Systems.Simulation.GameEntity.InteractableActions;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Simulation.Unit.InteractableActions
{
    [UpdateInGroup(typeof(ActionUIsHandleSystemGroup))]
    public partial class SpawnRecruitActionPanelSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ActionsContainerUIHolder
                    , PrimaryPrefabEntityHolder
                    , CanShowActionsContainerUITag
                    , ActionsContainerUIShownTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<UIPrefabAndPoolMap>();
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<EntityToContainerIndexMap>();
            this.RequireForUpdate<EntitySpawningCostsContainer>();
            this.RequireForUpdate<ResourceProfilesSOHolder>();
        }

        protected override void OnUpdate()
        {
            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var entitySpawningCostsContainer = SystemAPI.GetSingleton<EntitySpawningCostsContainer>();
            var resourceProfiles = SystemAPI.GetSingleton<ResourceProfilesSOHolder>().Value.Value.Profiles;

            foreach (var (factionIndexRef, actionsContainerUIHolderRef, primaryPrefabEntityHolderRef, canShowUITag, uiShownTag, entity) in SystemAPI
                .Query<
                    RefRO<FactionIndex>
                    , RefRO<ActionsContainerUIHolder>
                    , RefRO<PrimaryPrefabEntityHolder>
                    , EnabledRefRO<CanShowActionsContainerUITag>
                    , EnabledRefRO<ActionsContainerUIShownTag>>()
                .WithEntityAccess()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!canShowUITag.ValueRO) continue;
                if (uiShownTag.ValueRO) continue;
                if (factionIndexRef.ValueRO != FactionIndex.Neutral) continue;

                float3 spawnPos = actionsContainerUIHolderRef.ValueRO.Value.Value.transform.position;

                var actionPanelCtrl = (RecruitActionPanelCtrl)UISpawningHelper.Spawn(
                        uiPrefabAndPoolMap.Value
                        , spawnedUIMap.Value
                        , UIType.ActionPanel_Recruit
                        , spawnPos);

                int containerIndex = entityToContainerIndexMap.Value[primaryPrefabEntityHolderRef.ValueRO];
                int lowerBound = containerIndex * ResourceType_Length.Value;
                int upperBound = (containerIndex * ResourceType_Length.Value) + ResourceType_Length.Value;

                for (int i = lowerBound; i < upperBound; i++)
                {
                    uint cost = entitySpawningCostsContainer.Value[i];
                    if (cost == 0) continue;

                    ResourceType resourceType = (ResourceType)(i - containerIndex * ResourceType_Length.Value);

                    var costViewCtrl = (RecruitActionCostViewCtrl)UISpawningHelper.Spawn(
                        uiPrefabAndPoolMap.Value
                        , spawnedUIMap.Value
                        , UIType.RecruitActionCostView
                        , spawnPos);

                    costViewCtrl.CostText.TextMeshProUGUI.text = $"{cost}";

                    var resourceProfileId = new ResourceProfileId
                    {
                        ResourceType = resourceType,
                        VariantIndex = 0,
                    };

                    costViewCtrl.ResourceIcon.Image.sprite = resourceProfiles[resourceProfileId].ProfilePicture;

                    costViewCtrl.gameObject.SetActive(true);
                    actionPanelCtrl.RecruitActionCostViewsHolder.Add(costViewCtrl);

                }

                actionPanelCtrl.Initialize(in entity, 0, actionsContainerUIHolderRef.ValueRO.Value);
                actionPanelCtrl.gameObject.SetActive(true);
                actionsContainerUIHolderRef.ValueRO.Value.Value.ActionPanelsHolder.Add(actionPanelCtrl);

            }

        }

    }

}
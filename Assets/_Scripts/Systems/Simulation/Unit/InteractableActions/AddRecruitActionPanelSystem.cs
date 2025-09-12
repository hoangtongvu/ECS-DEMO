using Components.GameEntity;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.GameEntity.InteractableActions;
using Components.GameEntity.Misc;
using Components.GameResource;
using Components.UI.Pooling;
using Components.Unit.Recruit;
using Core.GameResource;
using Core.UI.Identification;
using Core.UI.InteractableActionsPanel.ActionPanel.RecruitActionPanel;
using Core.UI.InteractableActionsPanel.ActionPanel.RecruitActionPanel.RecruitActionCostView;
using Core.UI.Pooling;
using Systems.Simulation.GameEntity.InteractableActions;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Simulation.Unit.InteractableActions
{
    [UpdateInGroup(typeof(ActionsContainerUpdateSystemGroup))]
    public partial class AddRecruitActionPanelSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PrimaryPrefabEntityHolder>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<EntityToContainerIndexMap>();
            this.RequireForUpdate<EntitySpawningCostsContainer>();
            this.RequireForUpdate<ResourceProfilesSOHolder>();
            this.RequireForUpdate<UIPoolMapInitializedTag>();
        }

        protected override void OnUpdate()
        {
            if (!this.CanActionsContainerUpdate()) return;

            var actionsContainerUICtrl = SystemAPI.GetSingleton<ActionsContainerUI_CD.Holder>().Value.Value;
            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var entitySpawningCostsContainer = SystemAPI.GetSingleton<EntitySpawningCostsContainer>();
            var resourceProfiles = SystemAPI.GetSingleton<ResourceProfilesSOHolder>().Value.Value.Profiles;

            foreach (var (factionIndexRef, primaryPrefabEntityHolderRef, entity) in SystemAPI
                .Query<
                    RefRO<FactionIndex>
                    , RefRO<PrimaryPrefabEntityHolder>>()
                .WithAll<CanBeRecruitedTag>()
                .WithAll<IsTargetForActionsContainerUI>()
                .WithEntityAccess())
            {
                if (factionIndexRef.ValueRO != FactionIndex.Neutral) continue;

                float3 spawnPos = actionsContainerUICtrl.transform.position;

                var actionPanelCtrl = (RecruitActionPanelCtrl)UICtrlPoolMap.Instance
                        .Rent(UIType.ActionPanel_Recruit);
                actionPanelCtrl.transform.position = spawnPos;

                int containerIndex = entityToContainerIndexMap.Value[primaryPrefabEntityHolderRef.ValueRO];
                int lowerBound = containerIndex * ResourceType_Length.Value;
                int upperBound = (containerIndex * ResourceType_Length.Value) + ResourceType_Length.Value;

                for (int i = lowerBound; i < upperBound; i++)
                {
                    uint cost = entitySpawningCostsContainer.Value[i];
                    if (cost == 0) continue;

                    ResourceType resourceType = (ResourceType)(i - containerIndex * ResourceType_Length.Value);

                    var costViewCtrl = (RecruitActionCostViewCtrl)UICtrlPoolMap.Instance
                        .Rent(UIType.RecruitActionCostView);

                    costViewCtrl.transform.position = spawnPos;
                    costViewCtrl.CostText.TextMeshProUGUI.text = $"{cost}";

                    var resourceProfileId = new ResourceProfileId
                    {
                        ResourceType = resourceType,
                        VariantIndex = 0,
                    };

                    costViewCtrl.ResourceBGImage.Image.color = resourceProfiles[resourceProfileId].ResourceMainColor;

                    costViewCtrl.gameObject.SetActive(true);
                    actionPanelCtrl.RecruitActionCostViewsHolder.Add(costViewCtrl);

                }

                actionPanelCtrl.Initialize(in entity, 0, actionsContainerUICtrl);
                actionPanelCtrl.gameObject.SetActive(true);
                actionsContainerUICtrl.ActionPanelsHolder.Add(actionPanelCtrl);
            }

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
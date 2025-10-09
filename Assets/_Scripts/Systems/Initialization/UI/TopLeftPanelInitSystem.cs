using Core.GameResource;
using System;
using Unity.Entities;
using UnityEngine;
using Core.UI.Identification;
using Core.UI.TopLeftPanel;
using System.Collections.Generic;
using Core.UI.TopLeftPanel.ResourceDisplay;
using Components.GameEntity;
using Components.GameResource;
using AYellowpaper.SerializedCollections;
using Components.UI.Pooling;
using Core.UI.Pooling;

namespace Systems.Initialization.UI
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class TopLeftPanelInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    ResourceProfilesSOHolder
                    , BakedGameEntityProfileElement>()
                .Build();

            this.RequireForUpdate(this.query);
            this.RequireForUpdate<UIPoolMapInitializedTag>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var profiles = this.query.GetSingleton<ResourceProfilesSOHolder>().Value.Value.Profiles;

            this.SpawnTopLeftPanel(out var topLeftPanelManager);
            this.SpawnResourceDisplays(out var resourceDisplays);

            this.SetDisplayIcons(profiles, resourceDisplays);

            this.AddDisplaysIntoPanel(resourceDisplays, topLeftPanelManager);
        }

        private void SpawnTopLeftPanel(out TopLeftPanelManager topLeftPanelManager)
        {
            topLeftPanelManager = (TopLeftPanelManager)UICtrlPoolMap.Instance.Rent(UIType.TopLeftPanel);
            topLeftPanelManager.gameObject.SetActive(true);
        }

        private void SpawnResourceDisplays(out List<ResourceDisplayCtrl> resourceDisplays)
        {
            int length = Enum.GetNames(typeof(ResourceType)).Length;

            resourceDisplays = new();

            for (int i = 0; i < length; i++)
            {
                var resourceDisplay = (ResourceDisplayCtrl)UICtrlPoolMap.Instance.Rent(UIType.ResourceDisplay);
                resourceDisplay.gameObject.SetActive(true);
                resourceDisplays.Add(resourceDisplay);
            }
        }

        private void SetDisplayIcons(
            SerializedDictionary<ResourceProfileId, ResourceProfileElement> profiles
            , List<ResourceDisplayCtrl> resourceDisplays)
        {
            for (int i = 0; i < ResourceType_Length.Value; i++)
            {
                var resourceDisplay = resourceDisplays[i];
                var type = (ResourceType)i;

                var resourceProfileId = new ResourceProfileId
                {
                    ResourceType = type,
                    VariantIndex = 0,
                };

                resourceDisplay.ResourceDisplayData.ResourceType = type;

                if (!profiles.TryGetValue(resourceProfileId, out var resourceProfile))
                {
                    Debug.LogError($"Can't find profile for resourceType: {resourceProfileId}");
                    continue;
                }

                resourceDisplay.BackGroundImage.color = resourceProfile.ResourceMainColor;
            }

        }

        private void AddDisplaysIntoPanel(
            List<ResourceDisplayCtrl> resourceDisplays
            , TopLeftPanelManager topLeftPanelManager)
        {
            resourceDisplays.ForEach(d => topLeftPanelManager.AddResourceDisplay(d));
        }

    }

}
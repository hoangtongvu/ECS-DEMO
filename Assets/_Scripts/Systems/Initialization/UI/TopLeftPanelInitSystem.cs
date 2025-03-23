using Core.GameResource;
using System;
using Unity.Entities;
using UnityEngine;
using Utilities.Helpers;
using Components.ComponentMap;
using Core.UI.Identification;
using Core.UI.TopLeftPanel;
using System.Collections.Generic;
using Core.UI.TopLeftPanel.ResourceDisplay;

namespace Systems.Initialization.UI
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class TopLeftPanelInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<UIPrefabAndPoolMap>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();


            this.SpawnTopLeftPanel(
                uiPrefabAndPoolMap
                , spawnedUIMap
                , out var topLeftPanelManager);


            this.SpawnResourceDisplays(
                uiPrefabAndPoolMap
                , spawnedUIMap
                , out var resourceDisplays);

            this.SetDisplayIcons(resourceDisplays);

            this.AddDisplaysIntoPanel(resourceDisplays, topLeftPanelManager);
        }


        private void SpawnTopLeftPanel(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , out TopLeftPanelManager topLeftPanelManager)
        {
            topLeftPanelManager = (TopLeftPanelManager)
                UISpawningHelper.Spawn(
                    uiPrefabAndPoolMap
                    , spawnedUIMap
                    , UIType.TopLeftPanel);
            topLeftPanelManager.gameObject.SetActive(true);
        }

        private void SpawnResourceDisplays(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , out List<ResourceDisplayCtrl> resourceDisplays)
        {
            int length = Enum.GetNames(typeof(ResourceType)).Length;

            resourceDisplays = new();

            for (int i = 0; i < length; i++)
            {
                var resourceDisplay = (ResourceDisplayCtrl) UISpawningHelper.Spawn(
                    uiPrefabAndPoolMap
                    , spawnedUIMap
                    , UIType.ResourceDisplay);

                resourceDisplay.gameObject.SetActive(true);
                resourceDisplays.Add(resourceDisplay);
            }
        }

        private void SetDisplayIcons(List<ResourceDisplayCtrl> resourceDisplays)
        {
            var profilesManagerSO = Resources.Load<ResourceProfilesManagerSO>("Misc/ResourceProfilesManager");

            if (profilesManagerSO == null)
            {
                Debug.LogError("Can't Load ResourceProfilesManagerSO");
                return;
            }

            var profileMap = profilesManagerSO.Profiles;

            int length = Enum.GetNames(typeof(ResourceType)).Length;

            for (int i = 0; i < length; i++)
            {
                var resourceDisplay = resourceDisplays[i];
                var type = (ResourceType)i;

                resourceDisplay.ResourceType = type;

                if (!profileMap.TryGetValue(type, out var resourceProfile))
                {
                    Debug.LogError($"Can't find profile for resourceType: {type}");
                    continue;
                }

                resourceDisplay.ResourceImage.Image.sprite = resourceProfile.ResourceIcon;
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
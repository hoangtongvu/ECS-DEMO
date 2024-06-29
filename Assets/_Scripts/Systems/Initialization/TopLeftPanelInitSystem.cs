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


namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class TopLeftPanelInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<UIPoolMap>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPoolMap>();


            this.SpawnTopLeftPanel(
                uiPoolMap
                , spawnedUIMap
                , out var topLeftPanelManager);


            this.SpawnResourceDisplays(
                uiPoolMap
                , spawnedUIMap
                , out var resourceDisplays);

            this.SetDisplayIcons(resourceDisplays);

            this.AddDisplaysIntoPanel(resourceDisplays, topLeftPanelManager);
        }


        private void SpawnTopLeftPanel(
            UIPoolMap uiPoolMap
            , SpawnedUIMap spawnedUIMap
            , out TopLeftPanelManager topLeftPanelManager)
        {
            topLeftPanelManager = (TopLeftPanelManager)
                UISpawningHelper.Spawn(
                    uiPoolMap
                    , spawnedUIMap
                    , UIType.TopLeftPanel);
            topLeftPanelManager.gameObject.SetActive(true);
        }

        private void SpawnResourceDisplays(
            UIPoolMap uiPoolMap
            , SpawnedUIMap spawnedUIMap
            , out List<ResourceDisplayCtrl> resourceDisplays)
        {
            int length = Enum.GetNames(typeof(ResourceType)).Length;

            resourceDisplays = new();

            for (int i = 0; i < length; i++)
            {
                var resourceDisplay = (ResourceDisplayCtrl) UISpawningHelper.Spawn(
                    uiPoolMap
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
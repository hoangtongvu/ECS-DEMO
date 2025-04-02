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
using Components.GameEntity;
using Components.GameResource;
using Components;
using AYellowpaper.SerializedCollections;

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
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<UIPrefabAndPoolMap>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var profiles = this.query.GetSingleton<ResourceProfilesSOHolder>().Value.Value.Profiles;
            int resourceCount = SystemAPI.GetSingleton<EnumLength<ResourceType>>().Value;

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

            this.SetDisplayIcons(profiles, resourceCount, resourceDisplays);

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

        private void SetDisplayIcons(
            SerializedDictionary<ResourceProfileId, ResourceProfileElement> profiles
            , int resourceCount
            , List<ResourceDisplayCtrl> resourceDisplays)
        {
            for (int i = 0; i < resourceCount; i++)
            {
                var resourceDisplay = resourceDisplays[i];
                var type = (ResourceType)i;

                var resourceProfileId = new ResourceProfileId
                {
                    ResourceType = type,
                    VariantIndex = 0,
                };

                resourceDisplay.ResourceType = type;

                if (!profiles.TryGetValue(resourceProfileId, out var resourceProfile))
                {
                    Debug.LogError($"Can't find profile for resourceType: {resourceProfileId}");
                    continue;
                }

                resourceDisplay.ResourceImage.Image.sprite = resourceProfile.ProfilePicture;
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
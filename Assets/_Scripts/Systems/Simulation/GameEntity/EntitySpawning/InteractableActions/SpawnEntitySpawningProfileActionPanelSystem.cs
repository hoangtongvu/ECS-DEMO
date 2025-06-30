using Components.ComponentMap;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.GameEntity.InteractableActions;
using Components.GameEntity.Misc;
using Components.Misc.WorldMap.WorldBuilding.BuildingConstruction;
using Components.Player;
using Core.UI.Identification;
using Core.UI.InteractableActionsPanel.ActionPanel.EntitySpawningProfileActionPanel;
using Core.Utilities.Helpers;
using Systems.Simulation.GameEntity.InteractableActions;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Simulation.GameEntity.EntitySpawning.InteractableActions
{
    [UpdateInGroup(typeof(ActionUIsHandleSystemGroup))]
    public partial class SpawnEntitySpawningProfileActionPanelSystem : SystemBase
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
                    FactionIndex
                    , EntitySpawningProfileElement
                    , ActionsContainerUIHolder
                    , CanShowActionsContainerUITag
                    , ActionsContainerUIShownTag>()
                .WithNone<ConstructionRemaining>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate(this.playerQuery);
            this.RequireForUpdate<UIPrefabAndPoolMap>();
            this.RequireForUpdate<SpawnedUIMap>();
        }

        protected override void OnUpdate()
        {
            byte playerFactionIndex = this.playerQuery.GetSingleton<FactionIndex>().Value;

            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var spritesContainer = SystemAPI.GetSingleton<EntitySpawningSpritesContainer>();

            foreach (var (factionIndexRef, spawningProfiles, actionsContainerUIHolderRef, canShowUITag, uiShownTag, entity) in SystemAPI
                .Query<
                    RefRO<FactionIndex>
                    , DynamicBuffer<EntitySpawningProfileElement>
                    , RefRO<ActionsContainerUIHolder>
                    , EnabledRefRO<CanShowActionsContainerUITag>
                    , EnabledRefRO<ActionsContainerUIShownTag>>()
                .WithNone<ConstructionRemaining>()
                .WithEntityAccess()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!canShowUITag.ValueRO) continue;
                if (uiShownTag.ValueRO) continue;
                if (factionIndexRef.ValueRO.Value != playerFactionIndex) continue;

                float3 spawnPos = actionsContainerUIHolderRef.ValueRO.Value.Value.transform.position;

                int count = spawningProfiles.Length;

                for (int i = 0; i < count; i++)
                {
                    var profile = spawningProfiles[i];
                    var entityToSpawn = profile.PrefabToSpawn;

                    var actionPanelCtrl = (EntitySpawningProfileActionPanelCtrl)UISpawningHelper.Spawn(
                        uiPrefabAndPoolMap.Value
                        , spawnedUIMap.Value
                        , UIType.ActionPanel_EntitySpawningProfile
                        , spawnPos);

                    actionPanelCtrl.ProfilePic.Image.sprite = spritesContainer.Value[entityToContainerIndexMap.Value[entityToSpawn]];
                    actionPanelCtrl.SpawnCountText.SetSpawnCount(profile.SpawnCount.Value);
                    actionPanelCtrl.ProgressBar.ClearProgress();

                    actionPanelCtrl.Initialize(in entity, (sbyte)i, actionsContainerUIHolderRef.ValueRO.Value, i);
                    actionPanelCtrl.gameObject.SetActive(true);
                    actionsContainerUIHolderRef.ValueRO.Value.Value.ActionPanelsHolder.Add(actionPanelCtrl);
                }

            }

        }

    }

}
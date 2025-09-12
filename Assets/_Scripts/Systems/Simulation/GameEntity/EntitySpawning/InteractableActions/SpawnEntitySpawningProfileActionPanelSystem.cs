using Components.GameEntity.EntitySpawning;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.GameEntity.InteractableActions;
using Components.GameEntity.Misc;
using Components.Misc.WorldMap.WorldBuilding.BuildingConstruction;
using Components.Player;
using Components.UI.Pooling;
using Core.UI.Identification;
using Core.UI.InteractableActionsPanel.ActionPanel.EntitySpawningProfileActionPanel;
using Core.UI.Pooling;
using Systems.Simulation.GameEntity.InteractableActions;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Simulation.GameEntity.EntitySpawning.InteractableActions
{
    [UpdateInGroup(typeof(ActionsContainerUpdateSystemGroup))]
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
                    , EntitySpawningProfileElement>()
                .WithNone<ConstructionRemaining>()
                .WithAll<IsTargetForActionsContainerUI>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate(this.playerQuery);
            this.RequireForUpdate<UIPoolMapInitializedTag>();
        }

        protected override void OnUpdate()
        {
            if (!this.CanActionsContainerUpdate()) return;

            var actionsContainerUICtrl = SystemAPI.GetSingleton<ActionsContainerUI_CD.Holder>().Value.Value;
            byte playerFactionIndex = this.playerQuery.GetSingleton<FactionIndex>().Value;

            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var spritesContainer = SystemAPI.GetSingleton<EntitySpawningSpritesContainer>();

            foreach (var (factionIndexRef, spawningProfiles, entity) in SystemAPI
                .Query<
                    RefRO<FactionIndex>
                    , DynamicBuffer<EntitySpawningProfileElement>>()
                .WithNone<ConstructionRemaining>()
                .WithAll<IsTargetForActionsContainerUI>()
                .WithEntityAccess())
            {
                if (factionIndexRef.ValueRO.Value != playerFactionIndex) continue;

                float3 spawnPos = actionsContainerUICtrl.transform.position;

                int count = spawningProfiles.Length;

                for (int i = 0; i < count; i++)
                {
                    var profile = spawningProfiles[i];
                    var entityToSpawn = profile.PrefabToSpawn;

                    var actionPanelCtrl = (EntitySpawningProfileActionPanelCtrl)UICtrlPoolMap.Instance
                        .Rent(UIType.ActionPanel_EntitySpawningProfile);
                    actionPanelCtrl.transform.position = spawnPos;

                    actionPanelCtrl.ProfilePic.Image.sprite = spritesContainer.Value[entityToContainerIndexMap.Value[entityToSpawn]];
                    actionPanelCtrl.SpawnCountText.TrySetSpawnCount(profile.SpawnCount.Value);
                    actionPanelCtrl.ProgressBar.ClearProgress();

                    actionPanelCtrl.Initialize(in entity, (sbyte)i, actionsContainerUICtrl, i);
                    actionPanelCtrl.gameObject.SetActive(true);
                    actionsContainerUICtrl.ActionPanelsHolder.Add(actionPanelCtrl);
                }

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
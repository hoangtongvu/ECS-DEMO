using Components.GameEntity.EntitySpawning;
using Components.GameEntity.InteractableActions;
using Components.GameEntity.Misc;
using Components.Player;
using Components.UI.Pooling;
using Core.UI.Identification;
using Core.UI.InteractableActionsPanel.ActionPanel.DestroyEntityActionPanel;
using Core.UI.Pooling;
using Systems.Simulation.GameEntity.InteractableActions;
using Unity.Entities;
using Unity.Mathematics;

namespace Systems.Simulation.GameEntity.EntitySpawning.InteractableActions
{
    [UpdateInGroup(typeof(ActionsContainerUpdateSystemGroup))]
    public partial class SpawnDestroyEntityActionPanelSystem : SystemBase
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
                    EntitySpawningProfileElement>()
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

            foreach (var (factionIndexRef, entity) in SystemAPI
                .Query<
                    RefRO<FactionIndex>>()
                .WithAll<EntitySpawningProfileElement>()
                .WithAll<IsTargetForActionsContainerUI>()
                .WithEntityAccess())
            {
                if (factionIndexRef.ValueRO.Value != playerFactionIndex) continue;

                float3 spawnPos = actionsContainerUICtrl.transform.position;

                var actionPanelCtrl = (DestroyEntityActionPanelCtrl)UICtrlPoolMap.Instance
                    .Rent(UIType.ActionPanel_DestroyEntity);
                actionPanelCtrl.transform.position = spawnPos;

                actionPanelCtrl.Initialize(in entity, 123, actionsContainerUICtrl);
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
using Components.GameEntity;
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
    [UpdateInGroup(typeof(ActionUIsHandleSystemGroup))]
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
                    EntitySpawningProfileElement
                    , ActionsContainerUIHolder
                    , PrimaryPrefabEntityHolder
                    , CanShowActionsContainerUITag
                    , ActionsContainerUIShownTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate(this.playerQuery);
            this.RequireForUpdate<UIPoolMapInitializedTag>();
        }

        protected override void OnUpdate()
        {
            byte playerFactionIndex = this.playerQuery.GetSingleton<FactionIndex>().Value;

            foreach (var (factionIndexRef, actionsContainerUIHolderRef, primaryPrefabEntityHolderRef, canShowUITag, uiShownTag, entity) in SystemAPI
                .Query<
                    RefRO<FactionIndex>
                    , RefRO<ActionsContainerUIHolder>
                    , RefRO<PrimaryPrefabEntityHolder>
                    , EnabledRefRO<CanShowActionsContainerUITag>
                    , EnabledRefRO<ActionsContainerUIShownTag>>()
                .WithAll<EntitySpawningProfileElement>()
                .WithEntityAccess()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!canShowUITag.ValueRO) continue;
                if (uiShownTag.ValueRO) continue;
                if (factionIndexRef.ValueRO.Value != playerFactionIndex) continue;

                float3 spawnPos = actionsContainerUIHolderRef.ValueRO.Value.Value.transform.position;

                var actionPanelCtrl = (DestroyEntityActionPanelCtrl)UICtrlPoolMap.Instance
                    .Rent(UIType.ActionPanel_DestroyEntity);
                actionPanelCtrl.transform.position = spawnPos;

                actionPanelCtrl.Initialize(in entity, 123, actionsContainerUIHolderRef.ValueRO.Value);
                actionPanelCtrl.gameObject.SetActive(true);
                actionsContainerUIHolderRef.ValueRO.Value.Value.ActionPanelsHolder.Add(actionPanelCtrl);

            }

        }

    }

}
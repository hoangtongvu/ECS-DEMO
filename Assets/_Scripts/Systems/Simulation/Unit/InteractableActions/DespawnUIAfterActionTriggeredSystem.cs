using Components.ComponentMap;
using Components.GameEntity.InteractableActions;
using Unity.Entities;

namespace Systems.Simulation.Unit.InteractableActions
{
    [UpdateInGroup(typeof(ActionUIsHandleSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(ShowActionsContainerUISystem))]
    public partial class DespawnUIAfterActionTriggeredSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ActionsContainerUIHolder
                    , ActionsContainerUIShownTag
                    , NewlyActionTriggeredTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<UIPrefabAndPoolMap>();
            this.RequireForUpdate<SpawnedUIMap>();
        }

        protected override void OnUpdate()
        {
            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();

            foreach (var (actionsContainerUIHolderRef, uiShownTag) in SystemAPI
                .Query<
                    RefRW<ActionsContainerUIHolder>
                    , EnabledRefRW<ActionsContainerUIShownTag>>()
                .WithAll<NewlyActionTriggeredTag>())
            {
                this.DespawnActionsContainerAndAllActionPanels(
                    uiPrefabAndPoolMap
                    , spawnedUIMap
                    , ref actionsContainerUIHolderRef.ValueRW);

                uiShownTag.ValueRW = false;

            }

        }

        private void DespawnActionsContainerAndAllActionPanels(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , ref ActionsContainerUIHolder actionsContainerUIHolder)
        {
            var actionsContainerUICtrl = actionsContainerUIHolder.Value.Value;
            actionsContainerUICtrl.Despawn(uiPrefabAndPoolMap.Value, spawnedUIMap.Value);
            actionsContainerUIHolder.Value = null;
        }

    }

}
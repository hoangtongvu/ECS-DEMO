using Components.ComponentMap;
using Components.Unit.InteractableActions;
using Core.UI.Identification;
using Core.UI.InteractableActionsPanel;
using Core.Utilities.Extensions;
using Core.Utilities.Helpers;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation.Unit.InteractableActions
{
    [UpdateInGroup(typeof(ActionUIsHandleSystemGroup), OrderFirst = true)]
    public partial class ShowActionsContainerUISystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , ActionsContainerUIHolder
                    , CanShowActionsContainerUITag
                    , ActionsContainerUIShownTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<UIPrefabAndPoolMap>();
            this.RequireForUpdate<SpawnedUIMap>();
        }

        protected override void OnUpdate()
        {
            const float offsetY = 4f; // TODO: Get this value from else where
            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();

            foreach (var (unitTransform, actionsContainerUIHolderRef, canShowUITag, uiShownTag) in SystemAPI
                .Query<
                    RefRO<LocalTransform>
                    , RefRW<ActionsContainerUIHolder>
                    , EnabledRefRO<CanShowActionsContainerUITag>
                    , EnabledRefRO<ActionsContainerUIShownTag>>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!canShowUITag.ValueRO)
                {
                    if (uiShownTag.ValueRO)
                    {
                        this.DespawnActionsContainerAndAllActionPanels(
                            uiPrefabAndPoolMap
                            , spawnedUIMap
                            , ref actionsContainerUIHolderRef.ValueRW);
                    }

                    continue;
                }

                if (uiShownTag.ValueRO) continue;

                this.SpawnActionsContainerUI(
                    uiPrefabAndPoolMap
                    , spawnedUIMap
                    , unitTransform.ValueRO.Position.Add(y: offsetY)
                    , ref actionsContainerUIHolderRef.ValueRW);

            }

        }

        private void SpawnActionsContainerUI(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , in float3 spawnPos
            , ref ActionsContainerUIHolder actionsContainerUIHolder)
        {
            var baseUICtrl = UISpawningHelper.Spawn(
                uiPrefabAndPoolMap.Value
                , spawnedUIMap.Value
                , UIType.ActionsContainerUI
                , spawnPos);

            baseUICtrl.gameObject.SetActive(true);
            actionsContainerUIHolder.Value = (ActionsContainerUICtrl)baseUICtrl;
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
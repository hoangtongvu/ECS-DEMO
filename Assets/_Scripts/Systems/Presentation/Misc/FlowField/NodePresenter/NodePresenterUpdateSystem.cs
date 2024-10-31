using Unity.Entities;
using Unity.Burst;
using Components.ComponentMap;
using Core.UI.Identification;
using Core.UI.FlowField.GridNodePresenter;
using Components.Misc.FlowField;
using UnityEngine;
using Utilities.Helpers;
using Utilities.Extensions;

namespace Systems.Presentation.Misc.FlowField.NodePresenter
{

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(NodePresenterSpawnSystem))]
    [BurstCompile]
    public partial class NodePresenterUpdateSystem : SystemBase
    {

        protected override void OnCreate()
        {
            this.RequireForUpdate<FlowFieldGridMap>();
            this.RequireForUpdate<GridNodePresenterStartIndex>();
            this.RequireForUpdate<GridNodePresenterConfig>();
            this.RequireForUpdate<SpawnedUIMap>();

        }

        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;

            var flowFieldGridMap = SystemAPI.GetSingleton<FlowFieldGridMap>();
            var presenterStartIndex = SystemAPI.GetSingleton<GridNodePresenterStartIndex>();
            var gridNodePresenterConfig = SystemAPI.GetSingleton<GridNodePresenterConfig>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();

            flowFieldGridMap.LogMapBestCost();

            this.UpdateNodePresenters(
                spawnedUIMap
                , in flowFieldGridMap
                , in gridNodePresenterConfig
                , presenterStartIndex.Value);

        }

        private void UpdateNodePresenters(
            SpawnedUIMap spawnedUIMap
            , in FlowFieldGridMap flowFieldGridMap
            , in GridNodePresenterConfig presenterConfig
            , int presenterStartIndex)
        {
            int mapLength = flowFieldGridMap.Nodes.Length;

            for (int i = 0; i < mapLength; i++)
            {
                var node = flowFieldGridMap.Nodes[i];

                UIID presenterId = new()
                {
                    Type = UIType.GridNodePresenter,
                    LocalId = (uint) (i + presenterStartIndex),
                };

                if (!spawnedUIMap.Value.TryGetValue(presenterId, out var baseUICtrl))
                {
                    UnityEngine.Debug.LogError($"Can't find id: {presenterId} in spawnedUIMap");
                    return;
                }

                var presenterCtrl = (GridNodePresenterCtrl) baseUICtrl;

                FlowFieldGridHelper.SyncValuesToNodePresenter(in presenterConfig, presenterCtrl, in node);
                
            }

        }


    }


}
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Utilities.Helpers;
using Components.ComponentMap;
using Core.UI.Identification;
using Core.UI.FlowField.GridNodePresenter;
using Components.Misc.FlowField;
using Utilities;

namespace Systems.Presentation.Misc.FlowField.NodePresenter
{

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [BurstCompile]
    public partial class NodePresenterSpawnSystem : SystemBase
    {

        protected override void OnCreate()
        {
            this.RequireForUpdate<FlowFieldGridMap>();
            this.RequireForUpdate<GridDebugConfig>();
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<UIPoolMap>();

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddComponent<GridNodePresenterStartIndex>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var costMap = SystemAPI.GetSingleton<FlowFieldCostMap>();
            var flowFieldGridMap = SystemAPI.GetSingleton<FlowFieldGridMap>();
            int mapWidth = SystemAPI.GetSingleton<FlowFieldMapWidth>().Value;
            int2 gridOffset = SystemAPI.GetSingleton<MapGridOffset>().Value;
            float mapCellSize = SystemAPI.GetSingleton<MapCellSize>().Value;

            var presenterStartIndexRef = SystemAPI.GetSingletonRW<GridNodePresenterStartIndex>();
            var gridDebugConfig = SystemAPI.GetSingleton<GridDebugConfig>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPoolMap>();

            this.SpawnNodePresenters(
                spawnedUIMap
                , uiPoolMap
                , in flowFieldGridMap
                , in costMap
                , mapWidth
                , in gridOffset
                , ref presenterStartIndexRef.ValueRW
                , in gridDebugConfig
                , mapCellSize);

        }

        private void SpawnNodePresenters(
            SpawnedUIMap spawnedUIMap
            , UIPoolMap uiPoolMap
            , in FlowFieldGridMap flowFieldGridMap
            , in FlowFieldCostMap costMap
            , int mapWidth
            , in int2 gridOffset
            , ref GridNodePresenterStartIndex presenterStartIndex
            , in GridDebugConfig gridDebugConfig
            , float drawCellSize)
        {
            int mapLength = flowFieldGridMap.Nodes.Length;

            bool spawnedFirstPresenter = false;

            for (int i = 0; i < mapLength; i++)
            {
                var node = flowFieldGridMap.Nodes[i];
                var nodeCost = costMap.Value[i];

                FlowFieldGridHelper.MapIndexToGridPos(
                    mapWidth
                    , in gridOffset
                    , i
                    , out int x
                    , out int y);

                float3 center = new(
                        drawCellSize * x + drawCellSize / 2
                        , 0
                        , -(drawCellSize * y + drawCellSize / 2));


                var presenterCtrl = (GridNodePresenterCtrl)
                    UISpawningHelper.Spawn(
                        uiPoolMap
                        , spawnedUIMap
                        , UIType.GridNodePresenter
                        , center);

                FlowFieldGridHelper.SyncValuesToNodePresenter(in gridDebugConfig, presenterCtrl, in node, nodeCost);

                presenterCtrl.gameObject.SetActive(true);

                if (spawnedFirstPresenter) continue;

                spawnedFirstPresenter = true;
                presenterStartIndex.Value = (int)presenterCtrl.UIID.LocalId;

            }

        }

    }
}
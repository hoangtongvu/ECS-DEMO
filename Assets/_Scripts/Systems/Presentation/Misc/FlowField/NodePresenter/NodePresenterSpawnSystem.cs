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

            var flowFieldGridMap = SystemAPI.GetSingleton<FlowFieldGridMap>();
            int mapWidth = SystemAPI.GetSingleton<FlowFieldMapWidth>().Value;
            var presenterStartIndexRef = SystemAPI.GetSingletonRW<GridNodePresenterStartIndex>();
            var gridDebugConfig = SystemAPI.GetSingleton<GridDebugConfig>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPoolMap>();

            this.SpawnNodePresenters(
                spawnedUIMap
                , uiPoolMap
                , in flowFieldGridMap
                , mapWidth
                , ref presenterStartIndexRef.ValueRW
                , in gridDebugConfig
                , 0.5f);

        }

        private void SpawnNodePresenters(
            SpawnedUIMap spawnedUIMap
            , UIPoolMap uiPoolMap
            , in FlowFieldGridMap flowFieldGridMap
            , int mapWidth
            , ref GridNodePresenterStartIndex presenterStartIndex
            , in GridDebugConfig gridDebugConfig
            , float drawCellRadius)
        {
            int mapLength = flowFieldGridMap.Nodes.Length;
            int2 gridOffset = flowFieldGridMap.GridOffset;

            bool spawnedFirstPresenter = false;

            for (int i = 0; i < mapLength; i++)
            {
                var node = flowFieldGridMap.Nodes[i];

                FlowFieldGridHelper.MapIndexToGridPos(
                    mapWidth
                    , in gridOffset
                    , i
                    , out int x
                    , out int y);

                float3 center = new(
                        drawCellRadius * 2 * x + drawCellRadius
                        , 0.1f
                        , -(drawCellRadius * 2 * y + drawCellRadius));


                var presenterCtrl = (GridNodePresenterCtrl)
                    UISpawningHelper.Spawn(
                        uiPoolMap
                        , spawnedUIMap
                        , UIType.GridNodePresenter
                        , center);

                FlowFieldGridHelper.SyncValuesToNodePresenter(in gridDebugConfig, presenterCtrl, in node);

                presenterCtrl.gameObject.SetActive(true);

                if (spawnedFirstPresenter) continue;

                spawnedFirstPresenter = true;
                presenterStartIndex.Value = (int)presenterCtrl.UIID.LocalId;

            }

        }

    }
}
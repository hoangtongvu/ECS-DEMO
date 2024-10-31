using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Utilities.Helpers;
using Components.ComponentMap;
using Core.UI.Identification;
using Core.UI.FlowField.GridNodePresenter;
using Components.Misc.FlowField;
using Utilities.Extensions;
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
            this.RequireForUpdate<GridNodePresenterConfig>();
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<UIPoolMap>();

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddComponent<GridNodePresenterStartIndex>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var flowFieldGridMap = SystemAPI.GetSingleton<FlowFieldGridMap>();
            var presenterStartIndexRef = SystemAPI.GetSingletonRW<GridNodePresenterStartIndex>();
            var gridNodePresenterConfig = SystemAPI.GetSingleton<GridNodePresenterConfig>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPoolMap>();

            this.SpawnNodePresenters(
                spawnedUIMap
                , uiPoolMap
                , in flowFieldGridMap
                , ref presenterStartIndexRef.ValueRW
                , in gridNodePresenterConfig
                , 0.5f);

        }

        private void SpawnNodePresenters(
            SpawnedUIMap spawnedUIMap
            , UIPoolMap uiPoolMap
            , in FlowFieldGridMap flowFieldGridMap
            , ref GridNodePresenterStartIndex presenterStartIndex
            , in GridNodePresenterConfig presenterConfig
            , float drawCellRadius)
        {
            int2 gridMapSize = new(flowFieldGridMap.MapWidth, flowFieldGridMap.GetMapHeight());

            bool spawnedFirstPresenter = false;

            for (int y = 0; y < gridMapSize.y; y++)
            {
                for (int x = 0; x < gridMapSize.x; x++)
                {
                    var node = flowFieldGridMap.GetNodeAt(x, y);

                    float3 center = new(
                        drawCellRadius * 2 * x + drawCellRadius
                        , 0.1f
                        , drawCellRadius * 2 * y + drawCellRadius);


                    var presenterCtrl = (GridNodePresenterCtrl)
                        UISpawningHelper.Spawn(
                            uiPoolMap
                            , spawnedUIMap
                            , UIType.GridNodePresenter
                            , center);

                    FlowFieldGridHelper.SyncValuesToNodePresenter(in presenterConfig, presenterCtrl, in node);

                    presenterCtrl.gameObject.SetActive(true);

                    if (spawnedFirstPresenter) continue;

                    spawnedFirstPresenter = true;
                    presenterStartIndex.Value = (int) presenterCtrl.UIID.LocalId;

                }
            }

        }

    }
}
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Utilities.Helpers;
using Components.ComponentMap;
using Core.UI.Identification;
using Components.Misc.WorldMap;
using Core.UI.WorldMapDebug;
using Utilities;

namespace Systems.Presentation.Misc.WorldMap
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [BurstCompile]
    public partial class CellPresenterSpawnSystem : SystemBase
    {

        protected override void OnCreate()
        {
            this.RequireForUpdate<WorldTileCostMap>();
            this.RequireForUpdate<MapDebugConfig>();
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<UIPoolMap>();

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddComponent<CellPresenterStartIndex>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var costMap = SystemAPI.GetSingleton<WorldTileCostMap>();
            int2 gridOffset = SystemAPI.GetSingleton<MapGridOffset>().Value;
            float mapCellSize = SystemAPI.GetSingleton<MapCellSize>().Value;

            var presenterStartIndexRef = SystemAPI.GetSingletonRW<CellPresenterStartIndex>();
            var debugConfig = SystemAPI.GetSingleton<MapDebugConfig>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPoolMap>();

            this.SpawnNodePresenters(
                spawnedUIMap
                , uiPoolMap
                , in costMap
                , costMap.Width /*mapWidth*/
                , in gridOffset
                , ref presenterStartIndexRef.ValueRW
                , in debugConfig
                , mapCellSize);

        }

        private void SpawnNodePresenters(
            SpawnedUIMap spawnedUIMap
            , UIPoolMap uiPoolMap
            , in WorldTileCostMap costMap
            , int mapWidth
            , in int2 gridOffset
            , ref CellPresenterStartIndex presenterStartIndex
            , in MapDebugConfig debugConfig
            , float drawCellSize)
        {
            int mapLength = costMap.Value.Length;

            bool spawnedFirstPresenter = false;

            for (int i = 0; i < mapLength; i++)
            {
                byte nodeCost = costMap.Value[i].Cost;

                WorldMapHelper.MapIndexToGridPos(
                    mapWidth
                    , in gridOffset
                    , i
                    , out int x
                    , out int y);

                float3 center = new(
                        drawCellSize * x + drawCellSize / 2
                        , 0
                        , -(drawCellSize * y + drawCellSize / 2));


                var presenterCtrl = (CellPresenterCtrl)
                    UISpawningHelper.Spawn(
                        uiPoolMap
                        , spawnedUIMap
                        , UIType.WorldMapCellPresenter
                        , center);

                WorldMapHelper.SyncValuesToNodePresenter(in debugConfig, presenterCtrl, nodeCost);

                presenterCtrl.gameObject.SetActive(true);

                if (spawnedFirstPresenter) continue;

                spawnedFirstPresenter = true;
                presenterStartIndex.Value = (int)presenterCtrl.UIID.LocalId;

            }

        }

    }

}
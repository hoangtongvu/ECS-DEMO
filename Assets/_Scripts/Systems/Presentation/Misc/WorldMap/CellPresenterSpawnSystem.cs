using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;
using Utilities.Helpers;
using Components.ComponentMap;
using Core.UI.Identification;
using Components.Misc.WorldMap;
using Core.UI.WorldMapDebug;
using Utilities;
using UnityEngine;

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
            this.RequireForUpdate<UIPrefabAndPoolMap>();

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddComponent<CellPresenterStartIndex>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var costMap = SystemAPI.GetSingleton<WorldTileCostMap>();
            int2 gridOffset = SystemAPI.GetSingleton<MapGridOffset>().Value;
            half cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;

            var presenterStartIndexRef = SystemAPI.GetSingletonRW<CellPresenterStartIndex>();
            var debugConfig = SystemAPI.GetSingleton<MapDebugConfig>();
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();

            this.SpawnNodePresenters(
                spawnedUIMap
                , uiPrefabAndPoolMap
                , in costMap
                , costMap.Width /*mapWidth*/
                , in gridOffset
                , ref presenterStartIndexRef.ValueRW
                , in debugConfig
                , cellRadius);

        }

        private void SpawnNodePresenters(
            SpawnedUIMap spawnedUIMap
            , UIPrefabAndPoolMap uiPrefabAndPoolMap
            , in WorldTileCostMap costMap
            , int mapWidth
            , in int2 gridOffset
            , ref CellPresenterStartIndex presenterStartIndex
            , in MapDebugConfig debugConfig
            , half drawCellRadius)
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
                        drawCellRadius * 2 * x + drawCellRadius
                        , 0
                        , -(drawCellRadius * 2 * y + drawCellRadius));


                var presenterCtrl = (CellPresenterCtrl)
                    UISpawningHelper.Spawn(
                        uiPrefabAndPoolMap
                        , spawnedUIMap
                        , UIType.WorldMapCellPresenter
                        , center);

                WorldMapHelper.SyncValuesToNodePresenter(in debugConfig, presenterCtrl, nodeCost);

                Vector2 originalSize = presenterCtrl.GetComponent<RectTransform>().sizeDelta;
                presenterCtrl.GetComponent<RectTransform>().sizeDelta *= drawCellRadius * 2;

                presenterCtrl.gameObject.SetActive(true);

                if (spawnedFirstPresenter) continue;

                spawnedFirstPresenter = true;
                presenterStartIndex.Value = (int)presenterCtrl.UIID.LocalId;

            }

        }

    }

}
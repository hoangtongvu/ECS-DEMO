using Components.Misc.WorldMap;
using Core.Misc.WorldMap;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Systems.Initialization.Misc.WorldMap
{
    [UpdateInGroup(typeof(MapGenerateSystemGroup))]
    public partial class TestMapInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<TestMapTag>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            this.LoadTestWorldMap();
        }

        private void LoadTestWorldMap()
        {
            // Load CSV text file from Resources folder
            TextAsset csvFile = Resources.Load<TextAsset>("Misc/WorldMap/TestData/TestMap16x9");
            if (csvFile == null)
            {
                Debug.LogError("CSV file not found in Resources!");
                return;
            }

            // Split file content by lines and parse rows
            var lines = csvFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            int mapWidth = lines[0].Split(',').Length;
            int mapHeight = lines.Length;

            var costMap = new NativeArray<Cell>(mapWidth * mapHeight, Allocator.Persistent);

            // Parse CSV and fill in the FlowFieldGridNode array
            for (int y = 0; y < mapHeight; y++)
            {
                var row = lines[y].Split(',');
                for (int x = 0; x < mapWidth; x++)
                {
                    byte cost = byte.Parse(row[x].Trim());
                    int arrayIndex = (y * mapWidth) + x;

                    costMap[arrayIndex] = new()
                    {
                        Cost = cost,
                        ChunkIndex = -1,
                    };
                }
            }

            half cellRadius = new(0.7f);
            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new CellRadius
                {
                    Value = cellRadius,
                });

            WorldMapHelper.GetGridOffset(mapWidth, mapHeight, out var gridOffset);

            //this.CreateMapSizeComponents(mapWidth, mapHeight);
            this.CreateMapOffsetComponent(in gridOffset);
            this.CreateCostMap(in costMap, mapWidth, mapHeight, in gridOffset);

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new WorldMapChangedTag
                {
                    Value = true,
                });

            this.CreateGround(mapWidth, mapHeight, in cellRadius);

        }

        private void CreateMapOffsetComponent(in int2 gridOffset)
        {
            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new MapGridOffset
                {
                    Value = gridOffset,
                });

        }

        private void CreateCostMap(in NativeArray<Cell> costs, int mapWidth, int mapHeight, in int2 offset)
        {
            var costMap = new WorldTileCostMap
            {
                Value = costs,
                Width = mapWidth,
                Height = mapHeight,
                Offset = offset,
            };

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(costMap);

        }

        private void CreateGround(int mapWidth, int mapHeight, in half cellRadius)
        {
            var planeGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
            planeGO.name = "Ground";

            float worldPosOffsetX = mapWidth % 2 == 0 ? 0 : cellRadius;
            float worldPosOffsetY = mapHeight % 2 == 0 ? 0 : -cellRadius;
            float mapScaleRatio = cellRadius * 2;

            planeGO.transform.position = new(worldPosOffsetX, 0f, worldPosOffsetY);
            planeGO.transform.localScale = new((float)mapWidth / 10 * mapScaleRatio, 1f, (float)mapHeight / 10 * mapScaleRatio);

        }

    }

}
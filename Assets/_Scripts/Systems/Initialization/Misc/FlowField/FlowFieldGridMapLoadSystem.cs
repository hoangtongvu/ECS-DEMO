using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Components.Misc.FlowField;
using Core.Misc.FlowField;
using Utilities;
using Utilities.Helpers;
using Utilities.Extensions;
using Unity.Mathematics;

namespace Systems.Initialization.Misc.FlowField
{
    public partial class FlowFieldGridMapLoadSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.LoadFlowFieldGridMap();
            this.Enabled = false;

        }

        protected override void OnUpdate() {}

        private void LoadFlowFieldGridMap()
        {
            // Load CSV text file from Resources folder
            TextAsset csvFile = Resources.Load<TextAsset>("Misc/FlowField/TestData/TestMap16x9");
            if (csvFile == null)
            {
                Debug.LogError("CSV file not found in Resources!");
                return;
            }

            // Split file content by lines and parse rows
            var lines = csvFile.text.Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
            int mapWidth = lines[0].Split(',').Length;
            int mapHeight = lines.Length;

            var nodes = new NativeArray<FlowFieldGridNode>(mapWidth * mapHeight, Allocator.Persistent);
            var costs = new NativeArray<byte>(mapWidth * mapHeight, Allocator.Persistent);

            // Parse CSV and fill in the FlowFieldGridNode array
            for (int y = 0; y < mapHeight; y++)
            {
                var row = lines[y].Split(',');
                for (int x = 0; x < mapWidth; x++)
                {
                    byte cost = byte.Parse(row[x].Trim());
                    int arrayIndex = y * mapWidth + x;

                    nodes[arrayIndex] = new FlowFieldGridNode
                    {
                        BestCost = ushort.MaxValue,
                    };

                    costs[arrayIndex] = cost;
                }
            }

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new MapCellSize
                {
                    Value = 1f,
                });

            FlowFieldGridHelper.GetGridOffset(mapWidth, mapHeight, out var gridOffset);

            this.CreateMapSizeComponents(mapWidth, mapHeight);
            this.CreateMapOffsetComponent(in gridOffset);
            this.CreateCostMap(in costs, mapWidth, mapHeight, in gridOffset);

            FlowFieldGridMap gridMap = new()
            {
                Nodes = nodes,
            };

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(gridMap);

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new TargetGridPos
                {
                    Value = new(2, 3),
                });

            this.CreateGround(mapWidth, mapHeight);

        }

        private void CreateMapSizeComponents(int mapWidth, int mapHeight)
        {
            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new FlowFieldMapWidth
                {
                    Value = mapWidth,
                });

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new FlowFieldMapHeight
                {
                    Value = mapHeight,
                });

        }

        private void CreateMapOffsetComponent(in int2 gridOffset)
        {
            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new MapGridOffset
                {
                    Value = gridOffset,
                });

        }

        private void CreateCostMap(in NativeArray<byte> costs, int mapWidth, int mapHeight, in int2 offset)
        {
            var costMap = new FlowFieldCostMap
            {
                Value = costs,
                Width = mapWidth,
                Height = mapHeight,
                Offset = offset,
            };

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(costMap);

        }

        private void CreateGround(int mapWidth, int mapHeight)
        {
            const float cellRadius = 0.5f; // Find a new way to get this value
            var planeGO = GameObject.CreatePrimitive(PrimitiveType.Plane);
            planeGO.name = "Ground";

            float worldPosOffsetX = mapWidth % 2 == 0 ? 0 : cellRadius;
            float worldPosOffsetY = mapHeight % 2 == 0 ? 0 : -cellRadius;

            planeGO.transform.position = new(worldPosOffsetX, 0f, worldPosOffsetY);
            planeGO.transform.localScale = new((float)mapWidth / 10, 1f, (float)mapHeight / 10);

        }

    }


}
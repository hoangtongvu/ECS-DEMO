using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Components.Misc.FlowField;
using Core.Misc.FlowField;
using Utilities;
using Utilities.Helpers;

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

            // Parse CSV and fill in the FlowFieldGridNode array
            for (int y = 0; y < mapHeight; y++)
            {
                var row = lines[y].Split(',');
                for (int x = 0; x < mapWidth; x++)
                {
                    byte cost = byte.Parse(row[x].Trim());
                    nodes[y * mapWidth + x] = new FlowFieldGridNode
                    {
                        Cost = cost,
                        BestCost = ushort.MaxValue,
                    };
                }
            }

            FlowFieldGridHelper.GetGridOffset(mapWidth, mapHeight, out var gridOffset);

            FlowFieldGridMap gridMap = new()
            {
                Nodes = nodes,
                MapWidth = mapWidth,
                GridOffset = gridOffset,
            };

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(gridMap);

            this.CreateGround(mapWidth, mapHeight);

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
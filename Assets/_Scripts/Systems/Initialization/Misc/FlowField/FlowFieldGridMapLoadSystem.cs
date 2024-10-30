using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Components.Misc.FlowField;
using Core.Misc.FlowField;
using Utilities;
using Utilities.Extensions;

namespace Systems.Initialization.Misc.FlowField
{
    public partial struct FlowFieldGridMapLoadSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            this.LoadFlowFieldGridMap(ref state);

        }

        public void OnUpdate(ref SystemState state) { }

        private void LoadFlowFieldGridMap(ref SystemState state)
        {
            // Load CSV text file from Resources folder
            TextAsset csvFile = Resources.Load<TextAsset>("Misc/FlowField/TestData/TestMap10x10"); // Ensure a "CostMap.csv" file is placed in a "Resources" folder
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

            FlowFieldGridMap gridMap = new()
            {
                Nodes = nodes,
                MapWidth = mapWidth
            };

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(gridMap);

            //this.LogMap(in gridMap);

        }

        private void LogMap(in FlowFieldGridMap map)
        {
            int width = map.MapWidth;
            int height = map.GetMapHeight();

            // Log line per line.
            for (int i = 0; i < height; i++)
            {
                string logContent = "";
                for (int j = 0; j < width; j++)
                {
                    logContent += "[";
                    logContent += map.GetNodeAt(j, i).Cost;
                    logContent += "] ";
                }

                UnityEngine.Debug.Log(logContent);

            }

        }


    }


}
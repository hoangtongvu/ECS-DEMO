using Core.Misc.FlowField;
using Utilities.Helpers;
using Components.Misc.FlowField;
using Unity.Mathematics;

namespace Utilities.Extensions
{
    public static class FlowFieldGridMapExtension
    {
        public static int GetMapHeight(this FlowFieldGridMap map, int mapWidth) => map.Nodes.Length / mapWidth;

        public static FlowFieldGridNode GetNodeAt(this FlowFieldGridMap map, int mapWidth, int2 pos) => GetNodeAt(map, mapWidth, pos.x, pos.y);

        public static FlowFieldGridNode GetNodeAt(this FlowFieldGridMap map, int mapWidth, int x, int y)
        {
            int mapIndex = FlowFieldGridHelper.GridPosToMapIndex(mapWidth, map.GridOffset, new(x, y));
            return map.Nodes[mapIndex];
        }

        public static void LogMapCost(this FlowFieldGridMap map, int mapWidth, int mapHeight)
        {
            // Log line per line.
            for (int i = 0; i < mapHeight; i++)
            {
                string logContent = "";
                for (int j = 0; j < mapWidth; j++)
                {
                    logContent += "[";
                    logContent += map.GetNodeAt(j, i).Cost;
                    logContent += "] ";
                }

                UnityEngine.Debug.Log(logContent);

            }

        }

        public static void LogMapBestCost(this FlowFieldGridMap map, int mapWidth, int mapHeight)
        {
            int2 gridOffset = map.GridOffset;

            // Log line per line.
            for (int y = gridOffset.y; y < mapHeight + gridOffset.y; y++)
            {
                string logContent = "";
                for (int x = gridOffset.x; x < mapWidth + gridOffset.x; x++)
                {
                    logContent += "[";
                    logContent += map.GetNodeAt(mapWidth, x, y).BestCost;
                    logContent += "] ";
                }

                UnityEngine.Debug.Log(logContent);

            }

        }


    }


}

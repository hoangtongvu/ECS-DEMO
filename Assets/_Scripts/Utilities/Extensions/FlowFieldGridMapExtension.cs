using Core.Misc.FlowField;
using Utilities.Helpers;
using Components.Misc.FlowField;
using Unity.Mathematics;

namespace Utilities.Extensions
{
    public static class FlowFieldGridMapExtension
    {
        public static int GetMapHeight(this FlowFieldGridMap map, int mapWidth) => map.Nodes.Length / mapWidth;

        public static FlowFieldGridNode GetNodeAt(this FlowFieldGridMap map, int mapWidth, in int2 gridOffset, int2 pos) =>
            GetNodeAt(map, mapWidth, in gridOffset, pos.x, pos.y);

        public static FlowFieldGridNode GetNodeAt(this FlowFieldGridMap map, int mapWidth, in int2 gridOffset, int x, int y)
        {
            int mapIndex = FlowFieldGridHelper.GridPosToMapIndex(mapWidth, in gridOffset, new(x, y));
            return map.Nodes[mapIndex];
        }

        public static void LogMapBestCost(this FlowFieldGridMap map, int mapWidth, int mapHeight, in int2 gridOffset)
        {
            // Log line per line.
            for (int y = gridOffset.y; y < mapHeight + gridOffset.y; y++)
            {
                string logContent = "";
                for (int x = gridOffset.x; x < mapWidth + gridOffset.x; x++)
                {
                    logContent += "[";
                    logContent += map.GetNodeAt(mapWidth, gridOffset, x, y).BestCost;
                    logContent += "] ";
                }

                UnityEngine.Debug.Log(logContent);

            }

        }


    }


}

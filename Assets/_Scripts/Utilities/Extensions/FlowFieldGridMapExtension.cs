using Core.Misc.FlowField;
using Utilities.Helpers;
using Components.Misc.FlowField;
using Unity.Mathematics;

namespace Utilities.Extensions
{
    public static class FlowFieldGridMapExtension
    {
        public static int GetMapHeight(this FlowFieldGridMap map) => map.Nodes.Length / map.MapWidth;

        public static FlowFieldGridNode GetNodeAt(this FlowFieldGridMap map, int2 pos) => GetNodeAt(map, pos.x, pos.y);

        public static FlowFieldGridNode GetNodeAt(this FlowFieldGridMap map, int x, int y)
        {
            int mapIndex = FlowFieldGridHelper.GridPosToMapIndex(map.MapWidth, map.GridOffset, new(x, y));
            return map.Nodes[mapIndex];
        }

        public static void LogMapCost(this FlowFieldGridMap map)
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

        public static void LogMapBestCost(this FlowFieldGridMap map)
        {
            int2 gridOffset = map.GridOffset;
            int width = map.MapWidth;
            int height = map.GetMapHeight();

            // Log line per line.
            for (int y = gridOffset.y; y < height + gridOffset.y; y++)
            {
                string logContent = "";
                for (int x = gridOffset.x; x < width + gridOffset.x; x++)
                {
                    logContent += "[";
                    logContent += map.GetNodeAt(x, y).BestCost;
                    logContent += "] ";
                }

                UnityEngine.Debug.Log(logContent);

            }

        }


    }


}

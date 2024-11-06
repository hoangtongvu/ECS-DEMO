using Utilities.Helpers;
using Components.Misc.FlowField;
using Unity.Mathematics;
using Unity.Burst;

namespace Utilities.Extensions
{
    [BurstCompile]
    public static class FlowFieldCostMapExtension
    {
        [BurstCompile]
        public static byte GetCostAt(this FlowFieldCostMap map, int2 pos) =>
            map.GetCostAt(pos.x, pos.y);

        [BurstCompile]
        public static byte GetCostAt(this FlowFieldCostMap map, int x, int y)
        {
            int mapIndex = FlowFieldGridHelper.GridPosToMapIndex(map.Width, in map.Offset, x, y);
            return map.Value[mapIndex];
        }

        [BurstCompile]
        public static bool IsNodePassable(this FlowFieldCostMap map, in int2 nodeGridPos) =>
            map.IsNodePassable(nodeGridPos.x, nodeGridPos.y);

        [BurstCompile]
        public static bool IsNodePassable(this FlowFieldCostMap map, int nodeGridPosX, int nodeGridPosY) =>
            map.GetCostAt(nodeGridPosX, nodeGridPosY) != byte.MaxValue;

        public static void LogCostMap(this FlowFieldCostMap map)
        {
            int2 gridOffset = map.Offset;

            // Log line per line.
            for (int y = gridOffset.y; y < map.Height + gridOffset.y; y++)
            {
                string logContent = "";
                for (int x = gridOffset.x; x < map.Width + gridOffset.x; x++)
                {
                    logContent += "[";
                    logContent += map.GetCostAt(x, y);
                    logContent += "] ";
                }

                UnityEngine.Debug.Log(logContent);

            }

        }



    }


}

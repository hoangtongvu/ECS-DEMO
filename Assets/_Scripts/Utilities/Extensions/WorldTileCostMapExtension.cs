using Utilities.Helpers;
using Unity.Mathematics;
using Unity.Burst;
using Components.Misc.WorldMap;
using Core.Misc.WorldMap;
using Core.Utilities.Extensions;

namespace Utilities.Extensions
{
    [BurstCompile]
    public static class WorldTileCostMapExtension
    {
        [BurstCompile]
        public static ref Cell GetRefCellAt(in this WorldTileCostMap map, in int2 pos) =>
            ref map.GetRefCellAt(pos.x, pos.y);

        [BurstCompile]
        public static ref Cell GetRefCellAt(in this WorldTileCostMap map, int x, int y)
        {
            int mapIndex = WorldMapHelper.GridPosToMapIndex(map.Width, in map.Offset, x, y);
            return ref map.Value.ElementAt(mapIndex);
        }
                
        [BurstCompile]
        public static void GetCellAt(in this WorldTileCostMap map, in int2 pos, out Cell cell) =>
            map.GetCellAt(pos.x, pos.y, out cell);

        [BurstCompile]
        public static void GetCellAt(in this WorldTileCostMap map, int x, int y, out Cell cell)
        {
            int mapIndex = WorldMapHelper.GridPosToMapIndex(map.Width, in map.Offset, x, y);
            cell = map.Value[mapIndex];
        }

        public static void LogCostMap(in this WorldTileCostMap map)
        {
            int2 gridOffset = map.Offset;

            // Log line per line.
            for (int y = gridOffset.y; y < map.Height + gridOffset.y; y++)
            {
                string logContent = "";
                for (int x = gridOffset.x; x < map.Width + gridOffset.x; x++)
                {
                    logContent += "[";
                    map.GetCellAt(x, y, out Cell cell);
                    logContent += cell.Cost;
                    logContent += "] ";
                }

                UnityEngine.Debug.Log(logContent);

            }

        }

    }

}

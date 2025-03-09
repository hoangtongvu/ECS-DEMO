using Unity.Mathematics;
using Unity.Burst;
using Core.UI.WorldMapDebug;
using UnityEngine;
using Components.Misc.WorldMap;
using Core.Misc.WorldMap;
using Utilities.Extensions;
using Core.Utilities.Extensions;

namespace Utilities.Helpers
{
    [BurstCompile]
    public static class WorldMapHelper
    {
        // Burst error BC1064 due to returning int2, can be fixed using out keyword.
        //[BurstCompile]
        public static int2 MapIndexToGridPos(int gridMapWidth, in int2 gridOffset, int mapIndex)
        {
            int x = mapIndex % gridMapWidth + gridOffset.x;     // Column index
            int y = mapIndex / gridMapWidth + gridOffset.y;     // Row index
            return new int2(x, y);
        }
        
        public static void MapIndexToGridPos(int gridMapWidth, in int2 gridOffset, int mapIndex, out int x, out int y)
        {
            x = mapIndex % gridMapWidth + gridOffset.x;     // Column index
            y = mapIndex / gridMapWidth + gridOffset.y;     // Row index
        }

        [BurstCompile]
        public static void GridPosToWorldPos(in half cellRadius, in int2 gridPos, out float3 worldPos)
        {
            float worldPosOffsetX = cellRadius;
            float worldPosOffsetY = -cellRadius;
            worldPos = new(gridPos.x * cellRadius * 2 + worldPosOffsetX, 0f, -gridPos.y * cellRadius * 2 + worldPosOffsetY);
        }

        [BurstCompile]
        public static void WorldPosToGridPos(in half cellRadius, in float3 worldPos, out int2 gridPos)
        {
            gridPos = new(
                (int)math.floor(worldPos.x / cellRadius / 2)
                , (int)math.floor(-worldPos.z / cellRadius / 2));
        }

        [BurstCompile]
        public static int GridPosToMapIndex(int gridMapWidth, in int2 gridOffset, in int2 gridPos) =>
            GridPosToMapIndex(gridMapWidth, in gridOffset, gridPos.x, gridPos.y);

        [BurstCompile]
        public static int GridPosToMapIndex(int gridMapWidth, in int2 gridOffset, int x, int y) =>
            (y - gridOffset.y) * gridMapWidth + (x - gridOffset.x);

        public static void SyncValuesToNodePresenter(
            in MapDebugConfig debugConfig
            , CellPresenterCtrl presenterCtrl
            , byte nodeCost)
        {
            SetPresenterColor(presenterCtrl, nodeCost);
            SetPresenterCosts(in debugConfig, presenterCtrl, nodeCost);
            //presenterCtrl.TargetMarkImage.Hide();

        }
        private static void SetPresenterCosts(
            in MapDebugConfig debugConfig
            , CellPresenterCtrl presenterCtrl
            , byte cost)
        {
            if (debugConfig.ShowCellCosts) presenterCtrl.CostText.SetCost(cost);
            else presenterCtrl.CostText.Clear();

        }

        private static void SetPresenterColor(
            CellPresenterCtrl presenterCtrl
            , byte nodeCost)
        {
            bool nodePassable = nodeCost != byte.MaxValue;

            if (!nodePassable)
            {
                presenterCtrl.BackgroundImage.SetColor(Color.red);
                return;
            }

            // Define a base color (e.g., green) and a target color (e.g., brown)
            Color baseColor = Color.white;
            Color targetColor = new(0.6f, 0.3f, 0.1f); // Brown color

            // Define the maximum cost for scaling purposes
            const float maxCost = 6f; // Adjust as needed

            // Interpolate between the base color and the target color based on the node's cost
            float t = Mathf.Clamp01((nodeCost - 1) / maxCost);
            Color interpolatedColor = Color.Lerp(baseColor, targetColor, t);

            presenterCtrl.BackgroundImage.SetColor(interpolatedColor);

        }

        [BurstCompile]
        public static bool IsValidGridPos(int mapWidth, int mapHeight, in int2 gridOffset, in int2 pos)
        {
            return IsValidGridPos(mapWidth, mapHeight, in gridOffset, pos.x, pos.y);
        }

        [BurstCompile]
        public static bool IsValidGridPos(int mapWidth, int mapHeight, in int2 gridOffset, int x, int y)
        {
            return x >= gridOffset.x && x < mapWidth + gridOffset.x &&
            y >= gridOffset.y && y < mapHeight + gridOffset.y;
        }

        [BurstCompile]
        public static void GetGridOffset(int mapWidth, int mapHeight, out int offsetX, out int offsetY)
        {
            offsetX = -mapWidth + (mapWidth + 1) / 2;
            offsetY = -mapHeight + (mapHeight + 1) / 2;
        }

        [BurstCompile]
        public static void GetGridOffset(int mapWidth, int mapHeight, out int2 offset)
        {
            GetGridOffset(mapWidth, mapHeight, out int offsetX, out int offsetY);
            offset = new(offsetX, offsetY);
        }

        /// <summary>
        /// Return the nearest passable cell around the input impassable cell.
        /// Radius limit of 5.
        /// </summary>
        [BurstCompile]
        public static void GetValidStartCell(
            in WorldTileCostMap costMap
            , ref int2 originalCellPos
            , out Cell validStartCell)
        {
            const int radiusAroundOriginalPosLimit = 5;
            int currentRadius = 1;
            validStartCell = default;

            while (currentRadius <= radiusAroundOriginalPosLimit)
            {
                for (int x = originalCellPos.x - currentRadius; x <= originalCellPos.x + currentRadius; x++)
                {
                    int y = originalCellPos.y - currentRadius;
                    if (CheckAndSetCell(in costMap, x, y, ref validStartCell, ref originalCellPos)) return;
                }

                for (int x = originalCellPos.x - currentRadius; x <= originalCellPos.x + currentRadius; x++)
                {
                    int y = originalCellPos.y + currentRadius;
                    if (CheckAndSetCell(in costMap, x, y, ref validStartCell, ref originalCellPos)) return;
                }

                for (int y = originalCellPos.y - currentRadius + 1; y <= originalCellPos.y + currentRadius - 1; y++)
                {
                    int x = originalCellPos.x - currentRadius;
                    if (CheckAndSetCell(in costMap, x, y, ref validStartCell, ref originalCellPos)) return;
                }

                for (int y = originalCellPos.y - currentRadius + 1; y <= originalCellPos.y + currentRadius - 1; y++)
                {
                    int x = originalCellPos.x + currentRadius;
                    if (CheckAndSetCell(in costMap, x, y, ref validStartCell, ref originalCellPos)) return;
                }

                currentRadius++;
            }

            throw new System.Exception($"{nameof(currentRadius)} reached limit but found no valid StartCell.");

            [BurstCompile]
            static bool CheckAndSetCell(
                in WorldTileCostMap costMap
                , int x
                , int y
                , ref Cell validStartCell
                , ref int2 validStartPos)
            {
                if (!IsValidGridPos(costMap.Width, costMap.Height, in costMap.Offset, x, y)) return false;

                costMap.GetCellAt(x, y, out Cell cell);
                if (!cell.IsPassable()) return false;

                validStartCell = cell;
                validStartPos = new(x, y);
                return true;
            }

        }

    }

}

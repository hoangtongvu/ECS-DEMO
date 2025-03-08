using Unity.Mathematics;
using Unity.Burst;
using Core.UI.WorldMapDebug;
using UnityEngine;
using Components.Misc.WorldMap;

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
        public static void GridPosToWorldPos(in int2 gridPos, out float3 worldPos)
        {
            const float cellRadius = 0.5f;
            float worldPosOffsetX = cellRadius;
            float worldPosOffsetY = -cellRadius;
            worldPos = new(gridPos.x + worldPosOffsetX, 0f, -gridPos.y + worldPosOffsetY);
        }

        [BurstCompile]
        public static void WorldPosToGridPos(in float3 worldPos, out int2 gridPos)
        {
            const float cellRadius = 0.5f;
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

    }

}

using Components.Misc.FlowField;
using Core.Misc.FlowField;
using Core.UI.FlowField.GridNodePresenter;
using UnityEngine;
using Unity.Mathematics;
using Utilities.Extensions;
using Unity.Burst;

namespace Utilities.Helpers
{

    [BurstCompile]
    public static class FlowFieldGridHelper
    {
        // Burst error BC1064 due to returning float2, can be fixed using out keyword.
        //[BurstCompile]
        public static float2 GetVectorFromDirection(SimpleDirection direction)
        {
            return direction switch
            {
                SimpleDirection.Top => new float2(0, 1),
                SimpleDirection.Bottom => new float2(0, -1),
                SimpleDirection.Left => new float2(-1, 0),
                SimpleDirection.Right => new float2(1, 0),
                SimpleDirection.TopLeft => math.normalize(new float2(-1, 1)),
                SimpleDirection.TopRight => math.normalize(new float2(1, 1)),
                SimpleDirection.BottomLeft => math.normalize(new float2(-1, -1)),
                SimpleDirection.BottomRight => math.normalize(new float2(1, -1)),
                _ => float2.zero,
            };

        }

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
        public static int GridPosToMapIndex(int gridMapWidth, in int2 gridOffset, in int2 gridPos) =>
            GridPosToMapIndex(gridMapWidth, in gridOffset, gridPos.x, gridPos.y);

        [BurstCompile]
        public static int GridPosToMapIndex(int gridMapWidth, in int2 gridOffset, int x, int y) =>
            (y - gridOffset.y) * gridMapWidth + (x - gridOffset.x);

        public static void SyncTargetNodeValuesToNodePresenter(
            in GridDebugConfig gridDebugConfig
            , GridNodePresenterCtrl presenterCtrl
            , in FlowFieldGridNode node)
        {
            SetPresenterColor(presenterCtrl, in node);
            SetPresenterCosts(in gridDebugConfig, presenterCtrl, in node);
            presenterCtrl.TargetMarkImage.Show();

        }

        public static void SyncValuesToNodePresenter(
            in GridDebugConfig gridDebugConfig
            , GridNodePresenterCtrl presenterCtrl
            , in FlowFieldGridNode node)
        {
            SetPresenterColor(presenterCtrl, in node);
            SetPresenterCosts(in gridDebugConfig, presenterCtrl, in node);
            SetPresenterVector(in gridDebugConfig, presenterCtrl, in node);
            presenterCtrl.TargetMarkImage.Hide();

        }

        private static void SetPresenterCosts(
            in GridDebugConfig gridDebugConfig
            , GridNodePresenterCtrl presenterCtrl
            , in FlowFieldGridNode node)
        {
            if (gridDebugConfig.ShowCost) presenterCtrl.CostText.SetCost(node.Cost);
            else presenterCtrl.CostText.Clear();

            if (gridDebugConfig.ShowBestCost) presenterCtrl.BestCostText.SetBestCost(node.BestCost);
            else presenterCtrl.BestCostText.Clear();

        }

        private static void SetPresenterVector(
            in GridDebugConfig gridDebugConfig
            , GridNodePresenterCtrl presenterCtrl
            , in FlowFieldGridNode node)
        {
            if (!gridDebugConfig.ShowDirectionVector)
            {
                presenterCtrl.DirectionImage.Hide();
                return;
            }

            presenterCtrl.DirectionImage.Show();
            presenterCtrl.DirectionImage.SetDirection(node.DirectionVector);

        }

        private static void SetPresenterColor(
            GridNodePresenterCtrl presenterCtrl
            , in FlowFieldGridNode node)
        {
            if (!node.IsPassable())
            {
                presenterCtrl.BackgroundImage.SetColor(Color.red);
                return;
            }

            // Define a base color (e.g., green) and a target color (e.g., brown)
            Color baseColor = Color.white;
            Color targetColor = new Color(0.6f, 0.3f, 0.1f); // Brown color

            // Define the maximum cost for scaling purposes
            const float maxCost = 6f; // Adjust as needed

            // Interpolate between the base color and the target color based on the node's cost
            float t = Mathf.Clamp01((node.Cost - 1) / maxCost);
            Color interpolatedColor = Color.Lerp(baseColor, targetColor, t);

            presenterCtrl.BackgroundImage.SetColor(interpolatedColor);

        }

        [BurstCompile]
        public static ushort GetBestCost(
            in FlowFieldGridNode currentNode
            , in FlowFieldGridNode neighborNode)
        {
            int newBestCost = currentNode.BestCost + neighborNode.Cost;
            if (newBestCost >= ushort.MaxValue) newBestCost = ushort.MaxValue;

            return (ushort)newBestCost;
        }

        [BurstCompile]
        public static bool IsReachableNeighborNode(
            in FlowFieldGridMap flowFieldGridMap
            , in int2 currentNodePos
            , in int2 neighborDir)
        {
            int xDir = neighborDir.x;
            int yDir = neighborDir.y;

            bool isUnitVector = xDir == 0 || yDir == 0;
            if (isUnitVector) return true;

            var firstStraightNode = flowFieldGridMap.GetNodeAt(currentNodePos.Add(addAmountX: xDir));
            if (firstStraightNode.IsPassable()) return true;

            var secondStraightNode = flowFieldGridMap.GetNodeAt(currentNodePos.Add(addAmountY: yDir));
            if (secondStraightNode.IsPassable()) return true;

            return false;

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

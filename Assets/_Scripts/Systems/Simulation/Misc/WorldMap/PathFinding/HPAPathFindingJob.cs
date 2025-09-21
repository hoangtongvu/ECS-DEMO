using Components.Misc.WorldMap;
using Components.Misc.WorldMap.ChunkInnerPathCost;
using Components.Misc.WorldMap.LineCaching;
using Components.Misc.WorldMap.PathFinding;
using Core.Misc.WorldMap;
using Core.Misc.WorldMap.ChunkInnerPathCost;
using Core.Misc.WorldMap.PathFinding;
using Core.Utilities.Extensions;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utilities.Extensions;
using Utilities.Helpers;
using Utilities.Helpers.Misc.WorldMap;
using Utilities.Helpers.Misc.WorldMap.ChunkInnerPathCost;

namespace Systems.Simulation.Misc.WorldMap.PathFinding
{
    [WithAll(typeof(CanFindPathTag))]
    [BurstCompile]
    public partial struct HPAPathFindingJob : IJobEntity
    {
        const int startNodeIndex = -1;
        const int nullNodeIndex = -2;

        [ReadOnly] public WorldTileCostMap CostMap;
        [ReadOnly] public CachedLines CachedLines;
        [ReadOnly] public ChunkIndexToExitIndexesMap ChunkIndexToExitIndexesMap;
        [ReadOnly] public ChunkExitIndexesContainer ChunkExitIndexesContainer;
        [ReadOnly] public ChunkExitsContainer ChunkExitsContainer;
        [ReadOnly] public InnerPathCostMap InnerPathCostMap;
        [ReadOnly] public half CellRadius;

        void Execute(
            in LocalTransform transform
            , in TargetPosForPathFinding targetPosForPathFinding
            , ref DynamicBuffer<WaypointElement> waypoints)
        {
            waypoints.Clear();

            WorldMapHelper.WorldPosToGridPos(in this.CellRadius, in transform.Position, out int2 startPos);
            WorldMapHelper.WorldPosToGridPos(in this.CellRadius, in targetPosForPathFinding.Value, out int2 endPos);

            NativeList<int2> path = this.GetPath(
                startPos
                , endPos
                , Allocator.Temp);

            int pathLength = path.Length;

            for (int i = pathLength - 1; i >= 0; i--)
            {
                waypoints.Add(new()
                {
                    Value = path[i],
                });
            }

            path.Dispose();
        }

        [BurstCompile]
        private NativeList<int2> GetPath(
            int2 startPos
            , int2 endPos
            , Allocator allocator)
        {
            NativeList<int2> path = new(10, allocator);

            if (!this.CostMap.TryGetCellAt(startPos, out Cell startCell) ||
                !this.CostMap.TryGetCellAt(endPos, out Cell endCell))
            {
                return path;
            }

            if (!startCell.IsPassable())
                WorldMapHelper.GetPassableCellAroundObstacle(in this.CostMap, ref startPos, out startCell);

            if (!endCell.IsPassable())
                WorldMapHelper.GetPassableCellAroundObstacle(in this.CostMap, ref endPos, out endCell);

            int startChunkIndex = startCell.ChunkIndex;
            int endChunkIndex = endCell.ChunkIndex;

            if (startChunkIndex == endChunkIndex)
            {
                path.Add(endPos);
                return path;
            }

            // 2 major steps
            // 1. Go from startCell to Destination Chunk
            this.FirstStep(
                ref path
                , in startPos
                , startChunkIndex
                , in endPos
                , endChunkIndex);

            // 2. From exit's innerCell of destination chunk, go directly to destination cell
            if (path.Length == 0 || !path[^1].Equals(endPos)) path.Add(endPos);

            return path;
        }

        [BurstCompile]
        private void FirstStep(
            ref NativeList<int2> path
            , in int2 startPos
            , int startChunkIndex
            , in int2 endPos
            , int destinationChunkIndex)
        {
            var openList = new NativePriorityQueue<Node>(15, Allocator.Temp);
            var closedList = new NativeHashSet<Node>(15, Allocator.Temp);
            var nodeList = new NativeHashMap<int, Node>(15, Allocator.Temp);

            Node startNode = new()
            {
                NodeIndex = startNodeIndex,
                ParentNodeIndex = nullNodeIndex,
            };

            closedList.Add(startNode);
            nodeList.Add(nullNodeIndex, startNode);

            // For case startCell
            ChunkExitHelper.GetExitIndexesFromChunk(
                in this.ChunkIndexToExitIndexesMap
                , in this.ChunkExitIndexesContainer
                , startChunkIndex
                , Allocator.Temp
                , out var exitIndexes);

            int exitIndexCount = exitIndexes.Length;

            PathCostComputer costComputer = new()
            {
                CostMap = this.CostMap,
                GlobalCachedLines = this.CachedLines,
                LocalCachedLines = new()
                {
                    Value = new(20, Allocator.Temp),
                },
                Pos0 = startPos,
            };

            for (int i = 0; i < exitIndexCount; i++)
            {
                int exitIndexInContainer = exitIndexes[i];
                var exit = this.ChunkExitsContainer.Value[exitIndexInContainer];

                bool isCurrentExitOrdered = ChunkExitHelper.GetUnsafeCellsInRightOrder(
                    in this.CostMap
                    , in exit
                    , startChunkIndex
                    , out _
                    , out int innerCellMapIndex
                    , out _
                    , out int outerCellMapIndex);

                int2 innerCellPos = WorldMapHelper.MapIndexToGridPos(this.CostMap.Width, in this.CostMap.Offset, innerCellMapIndex);
                int2 outerCellPos = WorldMapHelper.MapIndexToGridPos(this.CostMap.Width, in this.CostMap.Offset, outerCellMapIndex);

                costComputer.Pos1 = innerCellPos;

                float gCost = costComputer.GetCost();
                float heuristic = this.GetHeuristic(in outerCellPos, in endPos);
                float fCost = gCost + heuristic;

                openList.Add(new()
                {
                    NodeIndex = exitIndexInContainer,
                    ParentNodeIndex = startNodeIndex,
                    ChunkIndex = startChunkIndex,
                    IsExitOrdered = isCurrentExitOrdered,
                    GCost = gCost,
                    FCost = fCost,
                });
            }

            bool pathFound = false;
            Node nodeToTraceBack = default;

            while (openList.TryPop(out var currentNode))
            {
                int currentExitIndex = currentNode.NodeIndex;
                var currentExit = this.ChunkExitsContainer.Value[currentExitIndex];

                ChunkExitHelper.GetUnsafeCellsInRightOrder(
                    in this.CostMap
                    , in currentExit
                    , currentNode.ChunkIndex
                    , out _
                    , out _
                    , out Cell currentOuterCell
                    , out int currentOuterCellMapIndex);

                // Terminating condition: Reached destination chunk.
                bool reachedDestinationChunk = currentOuterCell.ChunkIndex == destinationChunkIndex;

                if (reachedDestinationChunk)
                {
                    pathFound = true;
                    nodeToTraceBack = currentNode;
                    break;
                }

                closedList.Add(currentNode);
                nodeList.Add(currentNode.NodeIndex, currentNode);

                var neighborNodes = this.GetNeighborNodes(
                    in currentOuterCell
                    , currentExitIndex
                    , in currentOuterCell
                    , Allocator.Temp);

                int neighborCount = neighborNodes.Length;
                for (int i = 0; i < neighborCount; i++)
                {
                    Node neighborNode = neighborNodes[i];
                    ChunkExit neighborExit = this.ChunkExitsContainer.Value[neighborNode.NodeIndex];

                    bool isNeighborExitOrdered = ChunkExitHelper.GetUnsafeCellsInRightOrder(
                        in this.CostMap
                        , in neighborExit
                        , neighborNode.ChunkIndex
                        , out _
                        , out int neighborInnerCellMapIndex
                        , out _
                        , out int neighborOuterCellMapIndex);

                    if (closedList.Contains(neighborNode)) continue;

                    float distanceBetweenNodes = this.GetCostBetweenNodes(
                        currentOuterCellMapIndex
                        , neighborInnerCellMapIndex);

                    float tentativeG = currentNode.GCost + distanceBetweenNodes;

                    neighborNode.IsExitOrdered = isNeighborExitOrdered;
                    neighborNode.ParentNodeIndex = currentNode.NodeIndex;
                    neighborNode.GCost = tentativeG;
                    float heuristic = this.GetHeuristic(neighborOuterCellMapIndex, in endPos);

                    neighborNode.FCost = neighborNode.GCost + heuristic;

                    if (!openList.Contains(neighborNode, out int firstIndex))
                        openList.Add(neighborNode);
                    else if (tentativeG < neighborNode.GCost)
                        openList[firstIndex] = neighborNode;

                }

                neighborNodes.Dispose();
            }

            if (pathFound) this.ReconstructPath(
                in path
                , in nodeList
                , nodeToTraceBack
                , in startPos);

            openList.Dispose();
            closedList.Dispose();
            nodeList.Dispose();
        }

        [BurstCompile]
        private NativeArray<Node> GetNeighborNodes(
            in Cell currentExitOuterCell
            , int currentExitIndex
            , in Cell currentOuterCell
            , Allocator allocator)
        {
            // Get exit range from chunk index
            // Exclude the input exit, create nodes from exits and return.
            ChunkExitHelper.GetExitIndexesFromChunk(
                in this.ChunkIndexToExitIndexesMap
                , in this.ChunkExitIndexesContainer
                , currentExitOuterCell.ChunkIndex
                , Allocator.Temp
                , out var exitIndexes);

            int length = exitIndexes.Length;
            int tempIndex = 0;

            var neighborNodes = new NativeArray<Node>(length - 1, allocator);
            int neighborChunkIndex = currentOuterCell.ChunkIndex;

            for (int i = 0; i < length; i++)
            {
                int neighborExitIndex = exitIndexes[i];

                if (neighborExitIndex == currentExitIndex) continue;
                neighborNodes[tempIndex] = new()
                {
                    NodeIndex = neighborExitIndex,
                    ChunkIndex = neighborChunkIndex,
                };
                tempIndex++;
            }

            exitIndexes.Dispose();
            return neighborNodes;
        }

        [BurstCompile]
        private float GetCostBetweenNodes(
            int currentOuterCellMapIndex
            , int neighborInnerCellMapIndex)
        {
            if (currentOuterCellMapIndex == neighborInnerCellMapIndex) return 0;

            var key = new InnerPathKey(currentOuterCellMapIndex, neighborInnerCellMapIndex);
            if (!this.InnerPathCostMap.Value.TryGetValue(key, out float cost))
                throw new KeyNotFoundException($"{nameof(InnerPathCostMap)} does not contain Key: {key}.");

            return cost;
        }

        [BurstCompile]
        private void ReconstructPath(
            in NativeList<int2> path
            , in NativeHashMap<int, Node> nodeList
            , Node currentNode
            , in int2 startPos)
        {
            // Remove duplicated way points due to same exits' inner cells of chunks whose size is 1x1.
            var antiDuplicatedHashSet = new NativeHashSet<int2>(nodeList.Count * 2, Allocator.Temp);

            while (currentNode.ParentNodeIndex != startNodeIndex)
            {
                var exit = this.ChunkExitsContainer.Value[currentNode.NodeIndex];

                int firstMapIndexToAdd;
                int secondMapIndexToAdd;

                (firstMapIndexToAdd, secondMapIndexToAdd) = currentNode.IsExitOrdered
                    ? (exit.SecondCellMapIndex, exit.FirstCellMapIndex)
                    : (exit.FirstCellMapIndex, exit.SecondCellMapIndex);

                int2 firstWayPoint = WorldMapHelper.MapIndexToGridPos(this.CostMap.Width, in this.CostMap.Offset, firstMapIndexToAdd);
                int2 secondWayPoint = WorldMapHelper.MapIndexToGridPos(this.CostMap.Width, in this.CostMap.Offset, secondMapIndexToAdd);

                if (!antiDuplicatedHashSet.Contains(firstWayPoint))
                {
                    path.Add(firstWayPoint);
                    antiDuplicatedHashSet.Add(firstWayPoint);
                }

                if (!antiDuplicatedHashSet.Contains(secondWayPoint))
                {
                    path.Add(secondWayPoint);
                    antiDuplicatedHashSet.Add(secondWayPoint);
                }

                currentNode = nodeList[currentNode.ParentNodeIndex];

            }

            path.Add(startPos);
            path.Reverse();
            antiDuplicatedHashSet.Dispose();
        }

        [BurstCompile]
        private float GetHeuristic(
            int cellMapIndex
            , in int2 endPos)
        {
            int2 currentPos = WorldMapHelper.MapIndexToGridPos(this.CostMap.Width, in this.CostMap.Offset, cellMapIndex);
            return this.GetHeuristic(in currentPos, in endPos);
        }

        [BurstCompile]
        private float GetHeuristic(in int2 pos0, in int2 pos1) => math.distance(pos0, pos1);

    }

}
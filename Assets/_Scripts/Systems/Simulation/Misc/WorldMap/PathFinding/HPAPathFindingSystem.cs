using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Core.Misc.WorldMap;
using Components.Misc.WorldMap;
using Utilities.Extensions;
using Utilities.Helpers.Misc.WorldMap.ChunkInnerPathCost;
using Components.Misc.WorldMap.LineCaching;
using Components.Misc.WorldMap.ChunkInnerPathCost;
using Core.Misc.WorldMap.PathFinding;
using Utilities.Helpers;
using Core.Misc.WorldMap.ChunkInnerPathCost;
using Utilities.Helpers.Misc.WorldMap;
using System.Collections.Generic;
using Core.Utilities.Extensions;
using Components.Misc.WorldMap.PathFinding;
using Components;
using Components.Unit.MyMoveCommand;
using Unity.Transforms;
using Utilities.Helpers.Misc;
using Components.Misc;

namespace Systems.Simulation.Misc.WorldMap.PathFinding
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct HPAPathFindingSystem : ISystem
    {
        const int startNodeIndex = -1;
        const int nullNodeIndex = -2;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldTileCostMap>();
            state.RequireForUpdate<CellPosRangeMap>();
            state.RequireForUpdate<CellPositionsContainer>();
            state.RequireForUpdate<ChunkIndexToExitIndexesMap>();
            state.RequireForUpdate<ChunkExitIndexesContainer>();
            state.RequireForUpdate<ChunkExitsContainer>();
            state.RequireForUpdate<InnerPathCostMap>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var costMap = SystemAPI.GetSingleton<WorldTileCostMap>();
            var cellPosRangeMap = SystemAPI.GetSingleton<CellPosRangeMap>();
            var cellPositionsContainer = SystemAPI.GetSingleton<CellPositionsContainer>();
            var chunkIndexToExitIndexesMap = SystemAPI.GetSingleton<ChunkIndexToExitIndexesMap>();
            var exitIndexesContainer = SystemAPI.GetSingleton<ChunkExitIndexesContainer>();
            var exitsContainer = SystemAPI.GetSingleton<ChunkExitsContainer>();
            var innerPathCostMap = SystemAPI.GetSingleton<InnerPathCostMap>();
            half cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;
            half defaultStopMoveWorldRadius = SystemAPI.GetSingleton<DefaultStopMoveWorldRadius>().Value;

            foreach (var (transformRef, moveCommandElementRef, currentWaypointRef, distanceToCurrentWaypointRef, waypoints, canMoveEntityTag, canFindPathTag, entity) in
                SystemAPI.Query<
                    RefRO<LocalTransform>
                    , RefRO<MoveCommandElement>
                    , RefRW<CurrentWorldWaypoint>
                    , RefRW<DistanceToCurrentWaypoint>
                    , DynamicBuffer<WaypointElement>
                    , EnabledRefRW<CanMoveEntityTag>
                    , EnabledRefRW<CanFindPathTag>>()
                    .WithEntityAccess()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!canFindPathTag.ValueRO) continue;
                canFindPathTag.ValueRW = false;
                canMoveEntityTag.ValueRW = true;

                waypoints.Clear();

                // Note: Init targetPos && distanceToTarget, make sure targetPos != transform.pos to prevent NaN at MoveDirectionFloat2 due to unsafe normalization.
                currentWaypointRef.ValueRW.Value = transformRef.ValueRO.Position.Add(x: defaultStopMoveWorldRadius);
                distanceToCurrentWaypointRef.ValueRW.Value = defaultStopMoveWorldRadius;

                var absoluteDistanceXZToTargetRef = SystemAPI.GetComponentRW<AbsoluteDistanceXZToTarget>(entity);
                AbsoluteDistanceXZToTargetHelper.SetDistance(
                    ref absoluteDistanceXZToTargetRef.ValueRW
                    , in transformRef.ValueRO.Position
                    , in moveCommandElementRef.ValueRO.Float3);

                WorldMapHelper.WorldPosToGridPos(in cellRadius, in transformRef.ValueRO.Position, out int2 startPos);
                WorldMapHelper.WorldPosToGridPos(in cellRadius, in moveCommandElementRef.ValueRO.Float3, out int2 endPos);

                NativeList<int2> path = this.GetPath(
                    in costMap
                    , in chunkIndexToExitIndexesMap
                    , in exitIndexesContainer
                    , in exitsContainer
                    , in cellPosRangeMap
                    , in cellPositionsContainer
                    , in innerPathCostMap
                    , startPos
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

        }

        [BurstCompile]
        private NativeList<int2> GetPath(
            in WorldTileCostMap costMap
            , in ChunkIndexToExitIndexesMap chunkIndexToExitIndexesMap
            , in ChunkExitIndexesContainer exitIndexesContainer
            , in ChunkExitsContainer exitsContainer
            , in CellPosRangeMap cellPosRangeMap
            , in CellPositionsContainer cellPositionsContainer
            , in InnerPathCostMap innerPathCostMap
            , int2 startPos
            , int2 endPos
            , Allocator allocator)
        {
            NativeList<int2> path = new(10, allocator);

            costMap.GetCellAt(startPos, out Cell startCell);
            costMap.GetCellAt(endPos, out Cell endCell);

            if (!startCell.IsPassable())
                WorldMapHelper.GetPassableCellAroundObstacle(in costMap, ref startPos, out startCell);

            if (!endCell.IsPassable())
                WorldMapHelper.GetPassableCellAroundObstacle(in costMap, ref endPos, out endCell);

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
                in costMap
                , in chunkIndexToExitIndexesMap
                , in exitIndexesContainer
                , in exitsContainer
                , in cellPosRangeMap
                , in cellPositionsContainer
                , in innerPathCostMap
                , ref path
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
            in WorldTileCostMap costMap
            , in ChunkIndexToExitIndexesMap chunkIndexToExitIndexesMap
            , in ChunkExitIndexesContainer chunkExitIndexesContainer
            , in ChunkExitsContainer exitsContainer
            , in CellPosRangeMap cellPosRangeMap
            , in CellPositionsContainer cellPositionsContainer
            , in InnerPathCostMap innerPathCostMap
            , ref NativeList<int2> path
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
                in chunkIndexToExitIndexesMap
                , in chunkExitIndexesContainer
                , startChunkIndex
                , Allocator.Temp
                , out var exitIndexes);

            int exitIndexCount = exitIndexes.Length;

            FinalCostComputer finalCostComputer = new()
            {
                CostMap = costMap,
                CellPosRangeMap = cellPosRangeMap,
                CellPositionsContainer = cellPositionsContainer,
                Pos0 = startPos,
            };

            for (int i = 0; i < exitIndexCount; i++)
            {
                int exitIndexInContainer = exitIndexes[i];
                var exit = exitsContainer.Value[exitIndexInContainer];

                bool isCurrentExitOrdered = ChunkExitHelper.GetUnsafeCellsInRightOrder(
                    in costMap
                    , in exit
                    , startChunkIndex
                    , out _
                    , out int innerCellMapIndex
                    , out _
                    , out int outerCellMapIndex);

                int2 innerCellPos = WorldMapHelper.MapIndexToGridPos(costMap.Width, in costMap.Offset, innerCellMapIndex);
                int2 outerCellPos = WorldMapHelper.MapIndexToGridPos(costMap.Width, in costMap.Offset, outerCellMapIndex);

                finalCostComputer.Pos1 = innerCellPos;

                float gCost = finalCostComputer.GetCost();
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
                var currentExit = exitsContainer.Value[currentExitIndex];

                ChunkExitHelper.GetUnsafeCellsInRightOrder(
                    in costMap
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
                    in chunkIndexToExitIndexesMap
                    , in chunkExitIndexesContainer
                    , in currentOuterCell
                    , currentExitIndex
                    , in currentOuterCell
                    , Allocator.Temp);

                int neighborCount = neighborNodes.Length;
                for (int i = 0; i < neighborCount; i++)
                {
                    Node neighborNode = neighborNodes[i];
                    ChunkExit neighborExit = exitsContainer.Value[neighborNode.NodeIndex];

                    bool isNeighborExitOrdered = ChunkExitHelper.GetUnsafeCellsInRightOrder(
                        in costMap
                        , in neighborExit
                        , neighborNode.ChunkIndex
                        , out _
                        , out int neighborInnerCellMapIndex
                        , out _
                        , out int neighborOuterCellMapIndex);

                    if (closedList.Contains(neighborNode)) continue;

                    float distanceBetweenNodes = this.GetCostBetweenNodes(
                        in innerPathCostMap
                        , currentOuterCellMapIndex
                        , neighborInnerCellMapIndex);

                    float tentativeG = currentNode.GCost + distanceBetweenNodes;

                    neighborNode.IsExitOrdered = isNeighborExitOrdered;
                    neighborNode.ParentNodeIndex = currentNode.NodeIndex;
                    neighborNode.GCost = tentativeG;
                    float heuristic = this.GetHeuristic(in costMap, neighborOuterCellMapIndex, in endPos);

                    neighborNode.FCost = neighborNode.GCost + heuristic;

                    if (!openList.Contains(neighborNode, out int firstIndex))
                        openList.Add(neighborNode);
                    else if (tentativeG < neighborNode.GCost)
                        openList[firstIndex] = neighborNode;

                }

                neighborNodes.Dispose();

            }

            if (pathFound) this.ReconstructPath(
                in costMap
                , in exitsContainer
                , in path
                , in nodeList
                , nodeToTraceBack
                , in startPos);

            openList.Dispose();
            closedList.Dispose();
            nodeList.Dispose();

        }

        [BurstCompile]
        private NativeArray<Node> GetNeighborNodes(
            in ChunkIndexToExitIndexesMap chunkIndexToExitIndexesMap
            , in ChunkExitIndexesContainer exitIndexesContainer
            , in Cell currentExitOuterCell
            , int currentExitIndex
            , in Cell currentOuterCell
            , Allocator allocator)
        {
            // Get exit range from chunk index
            // Exclude the input exit, create nodes from exits and return.
            ChunkExitHelper.GetExitIndexesFromChunk(
                in chunkIndexToExitIndexesMap
                , in exitIndexesContainer
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
            in InnerPathCostMap innerPathCostMap
            , int currentOuterCellMapIndex
            , int neighborInnerCellMapIndex)
        {
            if (currentOuterCellMapIndex == neighborInnerCellMapIndex) return 0;

            var key = new InnerPathKey(currentOuterCellMapIndex, neighborInnerCellMapIndex);
            if (!innerPathCostMap.Value.TryGetValue(key, out float cost))
                throw new KeyNotFoundException($"{nameof(InnerPathCostMap)} does not contain Key: {key}.");

            return cost;
        }

        [BurstCompile]
        private void ReconstructPath(
            in WorldTileCostMap costMap
            , in ChunkExitsContainer chunkExitsContainer
            , in NativeList<int2> path
            , in NativeHashMap<int, Node> nodeList
            , Node currentNode
            , in int2 startPos)
        {
            // Remove duplicated way points due to same exits' inner cells of chunks whose size is 1x1.
            var antiDuplicatedHashSet = new NativeHashSet<int2>(nodeList.Count * 2, Allocator.Temp);

            while (currentNode.ParentNodeIndex != startNodeIndex)
            {
                var exit = chunkExitsContainer.Value[currentNode.NodeIndex];

                int firstMapIndexToAdd;
                int secondMapIndexToAdd;

                (firstMapIndexToAdd, secondMapIndexToAdd) = currentNode.IsExitOrdered
                    ? (exit.SecondCellMapIndex, exit.FirstCellMapIndex)
                    : (exit.FirstCellMapIndex, exit.SecondCellMapIndex);

                int2 firstWayPoint = WorldMapHelper.MapIndexToGridPos(costMap.Width, in costMap.Offset, firstMapIndexToAdd);
                int2 secondWayPoint = WorldMapHelper.MapIndexToGridPos(costMap.Width, in costMap.Offset, secondMapIndexToAdd);

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
            in WorldTileCostMap costMap
            , int cellMapIndex
            , in int2 endPos)
        {
            int2 currentPos = WorldMapHelper.MapIndexToGridPos(costMap.Width, in costMap.Offset, cellMapIndex);
            return this.GetHeuristic(in currentPos, in endPos);
        }

        [BurstCompile]
        private float GetHeuristic(in int2 pos0, in int2 pos1) => math.distance(pos0, pos1);

    }

}
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Components.Misc.FlowField;
using Utilities.Extensions;
using Unity.Collections;
using Core.Misc.FlowField;
using Utilities.Helpers;
using Components;

namespace Systems.Simulation.Misc
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct IntegrationFieldGenSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    InputData
                    , FlowFieldGridMap>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var inputData = SystemAPI.GetSingleton<InputData>();
            if (!inputData.BackspaceButtonDown) return;


            var costMap = SystemAPI.GetSingleton<FlowFieldCostMap>();
            var flowFieldMapRef = SystemAPI.GetSingletonRW<FlowFieldGridMap>();
            int2 targetPos = SystemAPI.GetSingleton<TargetGridPos>().Value;
            int mapWidth = SystemAPI.GetSingleton<FlowFieldMapWidth>().Value;
            int mapHeight = SystemAPI.GetSingleton<FlowFieldMapHeight>().Value;
            int2 gridOffset = SystemAPI.GetSingleton<MapGridOffset>().Value;

            NativeQueue<NodeAndPos> nodeAndPosQueue = new(state.WorldUpdateAllocator);
            NativeHashSet<int2> reachedPosSet = new(200, state.WorldUpdateAllocator);
            NativeArray<NodeAndPos> neighborNodeAndPosArray = new(8, Allocator.Temp);
            NativeArray<int2> neighborDirectionOrders = this.GetNeighborDirectionOrders();

            var targetNode = flowFieldMapRef.ValueRO.GetNodeAt(mapWidth, in gridOffset, targetPos.x, targetPos.y);
            targetNode.BestCost = 0;

            int targetNodeIndex = FlowFieldGridHelper.GridPosToMapIndex(
                mapWidth
                , in gridOffset
                , targetPos);
            flowFieldMapRef.ValueRW.Nodes[targetNodeIndex] = targetNode;

            nodeAndPosQueue.Enqueue(new()
            {
                Node = targetNode,
                GridPos = targetPos,
            });

            reachedPosSet.Add(targetPos);

            while (nodeAndPosQueue.TryDequeue(out var currentNodeAndPos))
            {
                var currentNode = currentNodeAndPos.Node;

                this.GetNeighborNodeAndPosArray(
                    in flowFieldMapRef.ValueRO
                    , in costMap
                    , mapWidth
                    , mapHeight
                    , in gridOffset
                    , in reachedPosSet
                    , in neighborDirectionOrders
                    , ref neighborNodeAndPosArray
                    , currentNodeAndPos.GridPos
                    , out int neighborNodeCount);


                for (int i = 0; i < neighborNodeCount; i++)
                {
                    var neighborNodeAndPos = neighborNodeAndPosArray[i];
                    var neighborNode = neighborNodeAndPos.Node;
                    int2 neighborPos = neighborNodeAndPos.GridPos;
                    byte neighborCost = costMap.GetCostAt(neighborPos);

                    bool neighborNodePassable = FlowFieldGridHelper.IsNodePassable(neighborCost);
                    if (neighborNodePassable)
                    {
                        neighborNode.BestCost = FlowFieldGridHelper.GetBestCost(in currentNode, neighborCost);

                        // Update new best cost to node in flowFieldMapRef.ValueRO.
                        int neighborNodeIndex = FlowFieldGridHelper.GridPosToMapIndex(
                            mapWidth
                            , in gridOffset
                            , neighborPos);
                        flowFieldMapRef.ValueRW.Nodes[neighborNodeIndex] = neighborNode;

                        neighborNodeAndPos.Node = neighborNode;
                        nodeAndPosQueue.Enqueue(neighborNodeAndPos);

                    }

                    // Add neighbor into reachedPos hash set
                    reachedPosSet.Add(neighborPos);

                }

            }

            neighborNodeAndPosArray.Dispose();
            neighborDirectionOrders.Dispose();

        }

        [BurstCompile]
        private NativeArray<int2> GetNeighborDirectionOrders()
        {
            NativeArray<int2> orders = new(8, Allocator.Temp);
            orders[0] = new(0, -1);
            orders[1] = new(1, -1);
            orders[2] = new(1, 0);
            orders[3] = new(1, 1);
            orders[4] = new(0, 1);
            orders[5] = new(-1, 1);
            orders[6] = new(-1, 0);
            orders[7] = new(-1, -1);

            return orders;
        }

        [BurstCompile]
        private void GetNeighborNodeAndPosArray(
            in FlowFieldGridMap flowFieldGridMap
            , in FlowFieldCostMap costMap
            , int mapWidth
            , int mapHeight
            , in int2 gridOffset
            , in NativeHashSet<int2> reachedPos
            , in NativeArray<int2> neighborDirectionOrders
            , ref NativeArray<NodeAndPos> neighborNodeAndPosArray
            , int2 currentNodePos
            , out int neighborNodeCount)
        {
            int arrayIndex = 0;

            for (int i = 0; i < 8; i++)
            {
                int2 neighborDir = neighborDirectionOrders[i];
                int2 neighborNodePos = currentNodePos + neighborDir;

                if (reachedPos.Contains(neighborNodePos)) continue;

                bool isValidGridPos =
                    FlowFieldGridHelper.IsValidGridPos(mapWidth, mapHeight, in gridOffset, neighborNodePos);

                if (!isValidGridPos) continue;

                bool isReachableNeighborNode =
                    FlowFieldGridHelper.IsReachableNeighborNode(in costMap, in currentNodePos, in neighborDir);

                if (!isReachableNeighborNode) continue;

                var neighborNode = flowFieldGridMap.GetNodeAt(mapWidth, in gridOffset, neighborNodePos);

                neighborNodeAndPosArray[arrayIndex] = new()
                {
                    Node = neighborNode,
                    GridPos = neighborNodePos,
                };

                arrayIndex++;

            }

            neighborNodeCount = arrayIndex;

        }



    }
}
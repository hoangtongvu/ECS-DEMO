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


            var flowFieldMapRef = SystemAPI.GetSingletonRW<FlowFieldGridMap>();
            int mapWidth = SystemAPI.GetSingleton<FlowFieldMapWidth>().Value;

            int2 targetPos = new(2, 3); // Temp target Pos for now.
            flowFieldMapRef.ValueRW.TargetGridPos = targetPos;

            NativeQueue<NodeAndPos> nodeAndPosQueue = new(state.WorldUpdateAllocator);
            NativeHashSet<int2> reachedPosSet = new(200, state.WorldUpdateAllocator);
            NativeArray<NodeAndPos> neighborNodeAndPosArray = new(8, Allocator.Temp);
            NativeArray<int2> neighborDirectionOrders = this.GetNeighborDirectionOrders();

            var targetNode = flowFieldMapRef.ValueRO.GetNodeAt(targetPos.x, targetPos.y);
            targetNode.BestCost = 0;

            int targetNodeIndex = FlowFieldGridHelper.GridPosToMapIndex(
                mapWidth
                , flowFieldMapRef.ValueRO.GridOffset
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
                    , mapWidth
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

                    if (neighborNode.IsPassable())
                    {
                        neighborNode.BestCost = FlowFieldGridHelper.GetBestCost(in currentNode, in neighborNode);

                        // Update new best cost to node in flowFieldMapRef.ValueRO.
                        int neighborNodeIndex =FlowFieldGridHelper.GridPosToMapIndex(
                            mapWidth
                            , flowFieldMapRef.ValueRO.GridOffset
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
            , int mapWidth
            , in NativeHashSet<int2> reachedPos
            , in NativeArray<int2> neighborDirectionOrders
            , ref NativeArray<NodeAndPos> neighborNodeAndPosArray
            , int2 currentNodePos
            , out int neighborNodeCount)
        {
            int mapHeight = flowFieldGridMap.GetMapHeight(mapWidth);
            int2 gridOffset = flowFieldGridMap.GridOffset;

            int arrayIndex = 0;

            for (int i = 0; i < 8; i++)
            {
                int2 neighborDir = neighborDirectionOrders[i];
                int2 neighborNodePos = currentNodePos + neighborDir;

                if (reachedPos.Contains(neighborNodePos)) continue;

                bool isValidGridPos =
                    FlowFieldGridHelper.IsValidGridPos(mapWidth, mapHeight, in flowFieldGridMap.GridOffset, neighborNodePos);

                if (!isValidGridPos) continue;

                bool isReachableNeighborNode =
                    FlowFieldGridHelper.IsReachableNeighborNode(in flowFieldGridMap, mapWidth, in currentNodePos, in neighborDir);

                if (!isReachableNeighborNode) continue;

                var neighborNode = flowFieldGridMap.GetNodeAt(mapWidth, neighborNodePos);

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
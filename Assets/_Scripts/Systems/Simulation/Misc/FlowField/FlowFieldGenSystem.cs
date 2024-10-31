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
    public partial struct FlowFieldGenSystem : ISystem
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
            if (!inputData.EnterButtonDown) return;

            var flowFieldMap = SystemAPI.GetSingleton<FlowFieldGridMap>();
            NativeArray<NodeAndPos> neighborNodeAndPosArray = new(8, Allocator.Temp);
            NativeArray<int2> neighborDirectionOrders = this.GetNeighborDirectionOrders();

            int mapWidth = flowFieldMap.MapWidth;
            int mapHeight = flowFieldMap.GetMapHeight();

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    int2 currentNodePos = new(x, y);

                    this.GetNeighborNodeAndPosArray(
                        in flowFieldMap
                        , mapWidth
                        , mapHeight
                        , in neighborDirectionOrders
                        , ref neighborNodeAndPosArray
                        , currentNodePos
                        , out int neighborCount);

                    this.GetMinBestCostPos(
                        neighborNodeAndPosArray
                        , neighborCount
                        , out int2 minBestCostNodePos);

                    int2 directionVector = minBestCostNodePos - currentNodePos;

                    this.SetNodeDirectionFromVector(
                        ref flowFieldMap
                        , mapWidth
                        , currentNodePos
                        , directionVector);

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
            orders[1] = new(1, 0);
            orders[2] = new(0, 1);
            orders[3] = new(-1, 0);
            orders[4] = new(1, -1);
            orders[5] = new(1, 1);
            orders[6] = new(-1, 1);
            orders[7] = new(-1, -1);

            return orders;
        }

        [BurstCompile]
        private void GetNeighborNodeAndPosArray(
            in FlowFieldGridMap flowFieldGridMap
            , int mapWidth
            , int mapHeight
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

                bool isValidGridPos =
                    neighborNodePos.x >= 0 && neighborNodePos.x < mapWidth &&
                    neighborNodePos.y >= 0 && neighborNodePos.y < mapHeight;

                if (!isValidGridPos) continue;

                neighborNodeAndPosArray[arrayIndex] = new()
                {
                    Node = flowFieldGridMap.GetNodeAt(neighborNodePos),
                    GridPos = neighborNodePos,
                };

                arrayIndex++;

            }

            neighborNodeCount = arrayIndex;

        }

        [BurstCompile]
        private void GetMinBestCostPos(
            in NativeArray<NodeAndPos> neighborNodeAndPosArray
            , int neighborCount
            , out int2 minBestCostNodePos)
        {
            ushort minBestCost = neighborNodeAndPosArray[0].Node.BestCost;
            minBestCostNodePos = neighborNodeAndPosArray[0].GridPos;

            for (int i = 1; i < neighborCount; i++)
            {
                var neighborNodeAndPos = neighborNodeAndPosArray[i];
                if (minBestCost <= neighborNodeAndPos.Node.BestCost) continue;

                minBestCost = neighborNodeAndPos.Node.BestCost;
                minBestCostNodePos = neighborNodeAndPos.GridPos;

            }

        }

        [BurstCompile]
        private void SetNodeDirectionFromVector(
            ref FlowFieldGridMap flowFieldMap
            , int mapWidth
            , int2 nodeGridPos
            , int2 directionVector)
        {
            int currentNodeMapIndex = FlowFieldGridHelper.GridPosToMapIndex(mapWidth, nodeGridPos);
            var currentNode = flowFieldMap.Nodes[currentNodeMapIndex];

            currentNode.DirectionVector = this.GetDirectionFromVector(directionVector);
            flowFieldMap.Nodes[currentNodeMapIndex] = currentNode;
        }

        [BurstCompile]
        private SimpleDirection GetDirectionFromVector(int2 vector)
        {
            if (vector.Equals(new int2(0, 1))) return SimpleDirection.Top;
            if (vector.Equals(new int2(0, -1))) return SimpleDirection.Bottom;
            if (vector.Equals(new int2(-1, 0))) return SimpleDirection.Left;
            if (vector.Equals(new int2(1, 0))) return SimpleDirection.Right;
            if (vector.Equals(new int2(-1, 1))) return SimpleDirection.TopLeft;
            if (vector.Equals(new int2(1, 1))) return SimpleDirection.TopRight;
            if (vector.Equals(new int2(-1, -1))) return SimpleDirection.BottomLeft;
            if (vector.Equals(new int2(1, -1))) return SimpleDirection.BottomRight;

            return default;
        }

    }
}
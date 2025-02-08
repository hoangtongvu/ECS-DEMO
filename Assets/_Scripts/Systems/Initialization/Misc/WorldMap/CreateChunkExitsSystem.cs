using Components.Misc.WorldMap;
using Core.Misc.WorldMap;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Utilities.Extensions;
using Core.Utilities.Extensions;

namespace Systems.Initialization.Misc.WorldMap
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(ChunkComponentsInitSystem))]
    [UpdateAfter(typeof(TestMapInitSystem))] //Note: CreateChunksSystem needs to update after WorldMapChangedTag writer systems
    [BurstCompile]
    public partial struct CreateChunkExitsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldMapChangedTag>();
            state.RequireForUpdate<ChunkList>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            bool worldMapChanged = SystemAPI.GetSingleton<WorldMapChangedTag>().Value;
            if (!worldMapChanged) return;

            var chunkList = SystemAPI.GetSingleton<ChunkList>();
            var chunkExitsContainer = SystemAPI.GetSingleton<ChunkExitsContainer>();
            var chunkIndexToExitsMap = SystemAPI.GetSingleton<ChunkIndexToExitsMap>();
            var neighborCellDirections = SystemAPI.GetSingleton<NeighborCellDirections>();
            var costMap = SystemAPI.GetSingleton<WorldTileCostMap>();

            int length = chunkList.Value.Length;

            for (int i = 0; i < length; i++)
            {
                Chunk chunk = chunkList.Value[i];

                int rangeStartIndex = chunkExitsContainer.Value.Length;
                int totalExits = 0;

                NativeArray<int2> borderCellPositions = this.GetBorderCellPositions(chunk, Allocator.Temp);

                foreach (var cellPos in borderCellPositions)
                {
                    costMap.GetCellAt(in cellPos, out Cell cell);

                    this.TraverseNeighborsAndCreateExits(
                        in costMap
                        , in chunkExitsContainer
                        , in neighborCellDirections.Value
                        , in cellPos
                        , cell.ChunkIndex
                        , out int singleCellExitAmount);

                    totalExits += singleCellExitAmount;

                }

                chunkIndexToExitsMap.Value[i] = new ChunkExitRange()
                {
                    StartIndex = rangeStartIndex,
                    Amount = totalExits,
                };

            }

        }

        // TODO: Make this static testable method.
        [BurstCompile]
        private NativeArray<int2> GetBorderCellPositions(Chunk chunk, Allocator allocator)
        {
            int chunkSize = chunk.BottomRightCellPos.x - chunk.TopLeftCellPos.x + 1;

            NativeArray<int2> posArray;

            if (chunkSize == 1)
            {
                posArray = new NativeArray<int2>(1, allocator);
		        posArray[0] = chunk.TopLeftCellPos;
                return posArray;
            }

            int borderCellAmount = chunkSize * 2 + 2 * (chunkSize - 2);
            posArray = new NativeArray<int2>(borderCellAmount, allocator);
            int arrayIndex = 0;

            for (int x = chunk.TopLeftCellPos.x; x <= chunk.BottomRightCellPos.x; x++)
            {
                posArray[arrayIndex] = new(x, chunk.TopLeftCellPos.y);
                arrayIndex++;
            }

            for (int x = chunk.TopLeftCellPos.x; x <= chunk.BottomRightCellPos.x; x++)
            {
                posArray[arrayIndex] = new(x, chunk.BottomRightCellPos.y);
                arrayIndex++;
            }

            for (int y = chunk.TopLeftCellPos.y + 1; y <= chunk.BottomRightCellPos.y - 1; y++)
            {
                posArray[arrayIndex] = new(chunk.TopLeftCellPos.x, y);
                arrayIndex++;
            }

            for (int y = chunk.TopLeftCellPos.y + 1; y <= chunk.BottomRightCellPos.y - 1; y++)
            {
                posArray[arrayIndex] = new(chunk.BottomRightCellPos.x, y);
                arrayIndex++;
            }

            return posArray;

        }

        [BurstCompile]
        private void TraverseNeighborsAndCreateExits(
            in WorldTileCostMap costMap
            , in ChunkExitsContainer chunkExitsContainer
            , in NativeArray<int2> neighborCellDirections
            , in int2 cellPos
            , int cellChunkIndex
            , out int singleCellExitAmount)
        {
            singleCellExitAmount = 0;

            int2 topLeftMapCellPos = costMap.Offset;
            int2 bottomRightMapCellPos = costMap.Offset + new int2(costMap.Width - 1, costMap.Height - 1);

            NativeArray<bool> straightNeighborPassableStates = new(4, Allocator.Temp);

            this.TraverseStraightNeighbors(
                in costMap
                , in chunkExitsContainer
                , in neighborCellDirections
                , ref straightNeighborPassableStates
                , in topLeftMapCellPos
                , in bottomRightMapCellPos
                , in cellPos
                , cellChunkIndex
                , ref singleCellExitAmount);

            this.TraverseDiagonalNeighbors(
                in costMap
                , in chunkExitsContainer
                , in neighborCellDirections
                , ref straightNeighborPassableStates
                , in topLeftMapCellPos
                , in bottomRightMapCellPos
                , in cellPos
                , cellChunkIndex
                , ref singleCellExitAmount);

            straightNeighborPassableStates.Dispose();

        }

        [BurstCompile]
        private void TraverseStraightNeighbors(
            in WorldTileCostMap costMap
            , in ChunkExitsContainer chunkExitsContainer
            , in NativeArray<int2> neighborCellDirections
            , ref NativeArray<bool> straightNeighborPassableStates
            , in int2 topLeftMapCellPos
            , in int2 bottomRightMapCellPos
            , in int2 cellPos
            , int cellChunkIndex
            , ref int singleCellExitAmount)
        {
            for (int i = 0; i < 4; i++)
            {
                int2 neighborDir = neighborCellDirections[i];
                int2 neighborPos = cellPos + neighborDir;

                if (!IsValidCellInMap(in topLeftMapCellPos, in bottomRightMapCellPos, in neighborPos)) continue;

                costMap.GetCellAt(neighborPos, out Cell neighborCell);

                bool isNeighborPassable = neighborCell.IsPassable();

                straightNeighborPassableStates[i] = isNeighborPassable;

                if (!isNeighborPassable) continue;
                if (neighborCell.ChunkIndex == cellChunkIndex) continue;

                this.CreateExit(in chunkExitsContainer, in cellPos, in neighborPos);
                singleCellExitAmount++;

            }

        }

        [BurstCompile]
        private void TraverseDiagonalNeighbors(
            in WorldTileCostMap costMap
            , in ChunkExitsContainer chunkExitsContainer
            , in NativeArray<int2> neighborCellDirections
            , ref NativeArray<bool> straightNeighborPassableStates
            , in int2 topLeftMapCellPos
            , in int2 bottomRightMapCellPos
            , in int2 cellPos
            , int cellChunkIndex
            , ref int singleCellExitAmount)
        {
            for (int i = 4; i < 8; i++)
            {
                int2 neighborDir = neighborCellDirections[i];
                int2 neighborPos = cellPos + neighborDir;

                if (!IsValidCellInMap(in topLeftMapCellPos, in bottomRightMapCellPos, in neighborPos)) continue;

                costMap.GetCellAt(neighborPos, out Cell neighborCell);

                if (!neighborCell.IsPassable()) continue;
                if (neighborCell.ChunkIndex == cellChunkIndex) continue;
                if (!this.IsDiagonalCellReachable(i, in straightNeighborPassableStates)) continue;

                this.CreateExit(in chunkExitsContainer, in cellPos, in neighborPos);
                singleCellExitAmount++;

            }

        }

        [BurstCompile]
        private bool IsValidCellInMap(in int2 topLeftMapCellPos, in int2 bottomRightMapCellPos, in int2 pos)
        {
            return pos.x >= topLeftMapCellPos.x && pos.x <= bottomRightMapCellPos.x &&
            pos.y >= topLeftMapCellPos.y && pos.y < bottomRightMapCellPos.y;
        }

        [BurstCompile]
        private void CreateExit(
            in ChunkExitsContainer chunkExitsContainer
            , in int2 cellPos
            , in int2 neighborCellPos)
        {
            ChunkExit exit = new()
            {
                InnerCellPos = cellPos,
                NeighborCellPos = neighborCellPos,
            };

            chunkExitsContainer.Value.Add(exit);

        }

        [BurstCompile]
        private bool IsDiagonalCellReachable(
            int originalIndex
            , in NativeArray<bool> straightNeighborPassableStates)
        {
            int tempIndex = originalIndex - 3;

            bool firstStraightNodePassable = originalIndex == 7
                ? straightNeighborPassableStates[0]
                : straightNeighborPassableStates[tempIndex];
            if (firstStraightNodePassable) return true;

            bool secondStraightNodePassable = straightNeighborPassableStates[tempIndex - 1];
            if (secondStraightNodePassable) return true;

            return false;

        }

    }

}
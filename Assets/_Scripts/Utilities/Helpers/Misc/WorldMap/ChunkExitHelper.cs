using Components.Misc.WorldMap;
using Core.Misc.WorldMap;
using Unity.Burst;
using Unity.Collections;

namespace Utilities.Helpers.Misc.WorldMap
{
    [BurstCompile]
    public static class ChunkExitHelper
    {
        [BurstCompile]
        public static void GetExitsFromChunk(
            in ChunkIndexToExitIndexesMap chunkIndexToExitIndexesMap
            , in ChunkExitIndexesContainer chunkExitIndexesContainer
            , in ChunkExitsContainer chunkExitsContainer
            , int chunkIndex
            , Allocator allocator
            , out NativeArray<ChunkExit> exits)
        {
            var indexRange = chunkIndexToExitIndexesMap.Value[chunkIndex];
            int upperBound = indexRange.StartIndex + indexRange.Amount;

            exits = new NativeArray<ChunkExit>(indexRange.Amount, allocator);
            int tempIndex = 0;

            for (int i = indexRange.StartIndex; i < upperBound; i++)
            {
                int exitIndex = chunkExitIndexesContainer.Value[i];

                exits[tempIndex] = chunkExitsContainer.Value[exitIndex];
                tempIndex++;
            }

        }

        [BurstCompile]
        public static void GetExitIndexesFromChunk(
            in ChunkIndexToExitIndexesMap chunkIndexToExitIndexesMap
            , in ChunkExitIndexesContainer chunkExitIndexesContainer
            , int chunkIndex
            , Allocator allocator
            , out NativeArray<int> exitIndexes)
        {
            ChunkExitIndexRange indexRange = chunkIndexToExitIndexesMap.Value[chunkIndex];

            //try
            //{
            //    indexRange = chunkIndexToExitIndexesMap.Value[chunkIndex];
            //}
            //catch (System.IndexOutOfRangeException)
            //{
            //    throw new System.IndexOutOfRangeException($"chunkIndex = {chunkIndex} is out of range in container of '{chunkIndexToExitIndexesMap.Value.Length}' length");
            //}

            int upperBound = indexRange.StartIndex + indexRange.Amount;

            exitIndexes = new NativeArray<int>(indexRange.Amount, allocator);
            int tempIndex = 0;

            for (int i = indexRange.StartIndex; i < upperBound; i++)
            {
                exitIndexes[tempIndex] = chunkExitIndexesContainer.Value[i];
                tempIndex++;
            }

        }

        /// <summary>
        /// This function assumes that the cell belong to chunk is either FirstCell or SecondCell of exit.
        /// </summary>
        /// <returns><c>true</c> if exit is in right order with input <paramref name="chunkIndex"/></returns>
        [BurstCompile]
        public static bool GetUnsafeInnerCellFromExit(
            in WorldTileCostMap costMap
            , in ChunkExit exit
            , int chunkIndex
            , out Cell cell
            , out int cellMapIndex)
        {
            Cell cell0 = costMap.Value[exit.FirstCellMapIndex];
            if (cell0.ChunkIndex == chunkIndex)
            {
                cell = cell0;
                cellMapIndex = exit.FirstCellMapIndex;
                return true;
            }

            cell = costMap.Value[exit.SecondCellMapIndex];
            cellMapIndex = exit.SecondCellMapIndex;
            return false;

        }

        /// <summary>
        /// This function assumes that the cell belong to chunk is either FirstCell or SecondCell of exit.
        /// </summary>
        [BurstCompile]
        public static void GetUnsafeOuterCellFromExit(
            in WorldTileCostMap costMap
            , in ChunkExit exit
            , int chunkIndex
            , out Cell cell
            , out int cellMapIndex)
        {
            Cell cell0 = costMap.Value[exit.FirstCellMapIndex];
            if (cell0.ChunkIndex != chunkIndex)
            {
                cell = cell0;
                cellMapIndex = exit.FirstCellMapIndex;
                return;
            }

            cell = costMap.Value[exit.SecondCellMapIndex];
            cellMapIndex = exit.SecondCellMapIndex;

        }

        /// <summary>
        /// This function assumes that the cell belong to chunk is either FirstCell or SecondCell of exit.
        /// </summary>
        /// <returns><c>true</c> if exit is in right order with input <paramref name="chunkIndex"/></returns>
        [BurstCompile]
        public static bool GetUnsafeCellsInRightOrder(
            in WorldTileCostMap costMap
            , in ChunkExit exit
            , int chunkIndex
            , out Cell innerCell
            , out int innerCellMapIndex
            , out Cell outerCell
            , out int outerCellMapIndex)
        {
            Cell cell0 = costMap.Value[exit.FirstCellMapIndex];
            Cell cell1 = costMap.Value[exit.SecondCellMapIndex];

            if (cell0.ChunkIndex == chunkIndex)
            {
                (innerCell, innerCellMapIndex) = (cell0, exit.FirstCellMapIndex);
                (outerCell, outerCellMapIndex) = (cell1, exit.SecondCellMapIndex);
                return true;
            }

            (innerCell, innerCellMapIndex) = (cell1, exit.SecondCellMapIndex);
            (outerCell, outerCellMapIndex) = (cell0, exit.FirstCellMapIndex);
            return false;

        }

    }

}

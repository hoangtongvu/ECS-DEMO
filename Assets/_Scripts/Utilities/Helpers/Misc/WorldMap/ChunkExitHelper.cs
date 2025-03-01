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

        /// <summary>
        /// This function assumes that return cell is either FirstCell or SecondCell of exit.
        /// </summary>
        [BurstCompile]
        public static void GetUnsafeCellBelongToChunkFromExit(
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
                return;
            }

            cell = costMap.Value[exit.SecondCellMapIndex];
            cellMapIndex = exit.SecondCellMapIndex;

        }

    }

}

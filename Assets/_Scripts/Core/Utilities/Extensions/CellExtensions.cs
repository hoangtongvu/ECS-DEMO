using Unity.Burst;
using Core.Misc.WorldMap;

namespace Core.Utilities.Extensions
{
    [BurstCompile]
    public static class CellExtensions
    {
        [BurstCompile]
        public static bool BelongToChunk(in this Cell cell) =>
            cell.ChunkIndex != -1;

        [BurstCompile]
        public static bool IsPassable(in this Cell cell) =>
            cell.Cost != byte.MaxValue;

    }

}

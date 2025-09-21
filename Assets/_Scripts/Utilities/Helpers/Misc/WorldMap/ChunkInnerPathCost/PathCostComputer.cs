using Components.Misc.WorldMap;
using Components.Misc.WorldMap.LineCaching;
using Core.Misc.WorldMap.LineCaching;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Utilities.Extensions;

namespace Utilities.Helpers.Misc.WorldMap.ChunkInnerPathCost
{
    [BurstCompile]
    public struct PathCostComputer
    {
        public WorldTileCostMap CostMap;
        public CachedLines GlobalCachedLines;
        public CachedLines LocalCachedLines;
        public int2 Pos0;
        public int2 Pos1;

        [BurstCompile]
        public float GetCost()
        {
            int2 originalDelta = this.Pos1 - this.Pos0;
            bool2 isZeroDelta = originalDelta == int2.zero;

            if (IsSingleCell(in isZeroDelta)) return 0;

            if (IsStraightLine(in isZeroDelta))
                return this.GetStraightLineCost(in originalDelta, in isZeroDelta);

            // Diagonal line
            int2 tempDelta = math.abs(originalDelta);
            bool canSwap = TrySwapTempDelta(ref tempDelta);

            var lineCacheKey = new LineCacheKey
            {
                Delta = tempDelta,
            };

            var line = this.GetFromCacheOrCreateNewLine(
                in lineCacheKey
                , Allocator.Temp);

            int lineLength = line.Length;
            int totalCost = 0;

            // Get totalCost
            for (int i = 0; i < lineLength; i++)
            {
                int2 cellPos = line[i];
                this.TranslateLineCellPos(in originalDelta, in tempDelta, ref cellPos, canSwap);

                cellPos += this.Pos0;

                this.CostMap.GetCellAt(in cellPos, out var cell);
                totalCost += cell.Cost;
            }

            line.Dispose();

            float avgCost = (float)totalCost / lineLength;
            return avgCost * math.distance(this.Pos0, this.Pos1);
        }

        [BurstCompile]
        private static bool TrySwapTempDelta(ref int2 tempDelta)
        {
            if (tempDelta.x > tempDelta.y)
            {
                (tempDelta.x, tempDelta.y) = (tempDelta.y, tempDelta.x);
                return true;
            }

            return false;
        }

        [BurstCompile]
        private static bool IsSingleCell(in bool2 isZeroDelta) => isZeroDelta.x && isZeroDelta.y;

        [BurstCompile]
        private static bool IsStraightLine(in bool2 isZeroDelta) => isZeroDelta.x || isZeroDelta.y;

        [BurstCompile]
        private int GetStraightLineCost(
            in int2 originalDelta
            , in bool2 isZeroDelta)
        {
            int totalCost = 0;

            if (isZeroDelta.x)
            {
                int startY, endY;
                (startY, endY) = originalDelta.y > 0
                    ? (this.Pos0.y, this.Pos1.y)
                    : (this.Pos1.y, this.Pos0.y);

                for (int y = startY; y <= endY; y++)
                {
                    this.CostMap.GetCellAt(this.Pos0.x, y, out var cell);
                    totalCost += cell.Cost;
                }

                return totalCost;
            }

            int startX, endX;
            (startX, endX) = originalDelta.x > 0
                ? (this.Pos0.x, this.Pos1.x)
                : (this.Pos1.x, this.Pos0.x);

            for (int x = startX; x <= endX; x++)
            {
                this.CostMap.GetCellAt(x, this.Pos0.y, out var cell);
                totalCost += cell.Cost;
            }

            return totalCost;
        }

        [BurstCompile]
        private NativeArray<int2> GetFromCacheOrCreateNewLine(
            in LineCacheKey lineCacheKey
            , Allocator allocator)
        {
            bool canGetLineFromCache = TryGetLineFromCache(
                in lineCacheKey
                , allocator
                , out NativeArray<int2> line);

            if (canGetLineFromCache) return line;

            CreateBresenhamLine(
                lineCacheKey.Delta.x
                , lineCacheKey.Delta.y
                , allocator
                , out var newLine);

            this.AddNewLineToLocalCache(in lineCacheKey, in newLine);

            return newLine;
        }

        [BurstCompile]
        private bool TryGetLineFromCache(
            in LineCacheKey lineCacheKey
            , Allocator allocator
            , out NativeArray<int2> line)
        {
            if (this.GlobalCachedLines.Value.ContainsKey(lineCacheKey))
            {
                this.GetLineFromCache(in this.GlobalCachedLines, in lineCacheKey, allocator, out line);
                return true;
            }

            if (this.LocalCachedLines.Value.ContainsKey(lineCacheKey))
            {
                this.GetLineFromCache(in this.LocalCachedLines, in lineCacheKey, allocator, out line);
                return true;
            }

            line = default;
            return false;
        }

        [BurstCompile]
        private void GetLineFromCache(
            in CachedLines cachedLines
            , in LineCacheKey lineCacheKey
            , Allocator allocator
            , out NativeArray<int2> line)
        {
            int lineIndex = 0;
            int lineLength = cachedLines.Value.CountValuesForKey(lineCacheKey);
            line = new(lineLength, allocator);

            foreach (var pos2 in cachedLines.Value.GetValuesForKey(lineCacheKey))
            {
                line[lineIndex] = pos2;
                lineIndex++;
            }
        }

        /// <summary>
        /// 0 < deltaX < deltaY
        /// </summary>
        [BurstCompile]
        public static void CreateBresenhamLine(
            int deltaX
            , int deltaY
            , Allocator allocator
            , out NativeArray<int2> bresenhamLine)
        {
            int e = 2 * deltaX - deltaY;
            int x = 0;
            int y = 0;

            int y1 = y + deltaY;

            bresenhamLine = new(deltaY + 1, allocator);
            int cellIndex = 0;

            while (y <= y1)
            {
                bresenhamLine[cellIndex] = new int2(x, y);

                if (e > 0)
                {
                    x++;
                    e += 2 * (deltaX - deltaY);
                }
                else
                    e += 2 * deltaX;

                y++;
                cellIndex++;
            }
        }

        [BurstCompile]
        private void AddNewLineToLocalCache(in LineCacheKey lineCacheKey, in NativeArray<int2> newLine)
        {
            int length = newLine.Length;
            for (int i = 0; i < length; i++)
            {
                this.LocalCachedLines.Value.Add(lineCacheKey, newLine[i]);
            }
        }

        [BurstCompile]
        private void TranslateLineCellPos(
            in int2 originalDelta
            , in int2 tempDelta
            , ref int2 lineCellPos
            , bool canSwap)
        {
            if (canSwap)
                (lineCellPos.y, lineCellPos.x) = (lineCellPos.x, lineCellPos.y);

            lineCellPos.x *= canSwap
                ? originalDelta.x / tempDelta.y
                : originalDelta.x / tempDelta.x;

            lineCellPos.y *= canSwap
                ? originalDelta.y / tempDelta.x
                : originalDelta.y / tempDelta.y;
        }

    }

}

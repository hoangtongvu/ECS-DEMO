using Components.Misc.WorldMap;
using Core.Misc.WorldMap;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Utilities.Extensions;
using Core.Utilities.Extensions;

namespace Systems.Initialization.Misc.WorldMap
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(MapGenerateSystemGroup))] //Note: CreateChunksSystem needs to update after WorldMapChangedTag writer systems
    [UpdateAfter(typeof(ChunkComponentsInitSystem))]
    [BurstCompile]
    public partial struct CreateChunksSystem : ISystem
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
            var chunkIndexToExitIndexesMap = SystemAPI.GetSingleton<ChunkIndexToExitIndexesMap>();
            var costMap = SystemAPI.GetSingleton<WorldTileCostMap>();
            int2 gridMapSize = new(costMap.Width, costMap.Height);
            int2 gridOffset = SystemAPI.GetSingleton<MapGridOffset>().Value;

            int xBound = gridMapSize.x + gridOffset.x;
            int yBound = gridMapSize.y + gridOffset.y;

            for (int y = gridOffset.y; y < yBound; y++)
            {
                for (int x = gridOffset.x; x < xBound; x++)
                {
                    ref Cell cell = ref costMap.GetRefCellAt(x, y);

                    if (!cell.IsPassable() || cell.BelongToChunk()) continue;

                    int maxChunkSize = this.GetMaxChunkSizeFromTopLeftPos(in costMap, x, y, xBound, yBound);

                    chunkList.Value.Add(new()
                    {
                        TopLeftCellPos = new(x, y),
                        BottomRightCellPos = new(x + maxChunkSize - 1, y + maxChunkSize - 1),
                    });

                    int newChunkIndex = chunkList.Value.Length - 1;
                    this.AssignChunkIndexToCells(in costMap, new int2(x, y), maxChunkSize, newChunkIndex);

                }

            }

            this.ReCreateChunkIndexToExitsMap(in chunkIndexToExitIndexesMap, in chunkList);

        }

        [BurstCompile]
        private int GetMaxChunkSizeFromTopLeftPos(
            in WorldTileCostMap costMap
            , int x
            , int y
            , int xBound
            , int yBound)
        {
            int chunkSize = 1;

            while (true)
            {
                bool reachedMaxSize = false;

                //Note: This can be optimized furthermore.
                for (int tempY = y; tempY < y + chunkSize; tempY++)
                {
                    if (tempY >= yBound)
                    {
                        reachedMaxSize = true;
                        break;
                    }

                    for (int tempX = x; tempX < x + chunkSize; tempX++)
                    {
                        if (tempX >= xBound)
                        {
                            reachedMaxSize = true;
                            break;
                        }

                        costMap.GetCellAt(tempX, tempY, out Cell tempCell);

                        if (tempCell.IsPassable() && !tempCell.BelongToChunk()) continue;

                        reachedMaxSize = true;
                    }

                    if (reachedMaxSize) break;

                }

                if (reachedMaxSize)
                {
                    chunkSize--;
                    break;
                }

                chunkSize++;
            }

            return chunkSize;

        }

        [BurstCompile]
        private void AssignChunkIndexToCells(
            in WorldTileCostMap costMap
            , int2 topLeftPos
            , int chunkSize
            , int chunkIndex)
        {
            for (int tempY = topLeftPos.y; tempY < topLeftPos.y + chunkSize; tempY++)
            {
                for (int tempX = topLeftPos.x; tempX < topLeftPos.x + chunkSize; tempX++)
                {
                    ref Cell cell = ref costMap.GetRefCellAt(tempX, tempY);
                    cell.ChunkIndex = chunkIndex;
                }

            }

        }

        [BurstCompile]
        private void ReCreateChunkIndexToExitsMap(
            in ChunkIndexToExitIndexesMap chunkIndexToExitIndexesMap
            , in ChunkList chunkList)
        {
            chunkIndexToExitIndexesMap.Value.Clear();
            int addAmount = chunkList.Value.Length;

            chunkIndexToExitIndexesMap.Value.AddReplicate(new ChunkExitIndexRange
            {
                StartIndex = -1,
                Amount = 0,
            }, addAmount);

        }

    }

}
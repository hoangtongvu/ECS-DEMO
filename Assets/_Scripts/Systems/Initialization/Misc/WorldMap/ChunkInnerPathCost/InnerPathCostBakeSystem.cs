using Components.Misc.WorldMap;
using Components.Misc.WorldMap.ChunkInnerPathCost;
using Components.Misc.WorldMap.LineCaching;
using Core.Misc.WorldMap;
using Core.Misc.WorldMap.ChunkInnerPathCost;
using Core.Misc.WorldMap.LineCaching;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Utilities;
using Utilities.Helpers;
using Utilities.Helpers.Misc.WorldMap;
using Utilities.Helpers.Misc.WorldMap.ChunkInnerPathCost;

namespace Systems.Initialization.Misc.WorldMap.ChunkInnerPathCost
{
    [UpdateInGroup(typeof(MapComponentsProcessSystemGroup))]
    [UpdateAfter(typeof(CreateChunkExitsSystem))]
    [BurstCompile]
    public partial struct InnerPathCostBakeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldMapChangedTag>();
            state.RequireForUpdate<ChunkList>();

            var su = SingletonUtilities.GetInstance(state.EntityManager);

            su.AddOrSetComponentData(new CachedLines
            {
                Value = new(200, Allocator.Persistent),
            });

            su.AddOrSetComponentData(new InnerPathCostMap
            {
                Value = new(500, Allocator.Persistent),
            });
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            bool worldMapChanged = SystemAPI.GetSingleton<WorldMapChangedTag>().Value;
            if (!worldMapChanged) return;

            var costMap = SystemAPI.GetSingleton<WorldTileCostMap>();
            var cachedLines = SystemAPI.GetSingleton<CachedLines>();
            var chunkList = SystemAPI.GetSingleton<ChunkList>();
            var chunkIndexToExitIndexesMap = SystemAPI.GetSingleton<ChunkIndexToExitIndexesMap>();
            var chunkExitIndexesContainer = SystemAPI.GetSingleton<ChunkExitIndexesContainer>();
            var chunkExitsContainer = SystemAPI.GetSingleton<ChunkExitsContainer>();
            var innerPathCostMap = SystemAPI.GetSingleton<InnerPathCostMap>().Value;
            var highestExitCount = SystemAPI.GetSingleton<HighestExitCount>().Value;

            // TODO: Find another way to get this value
            // The cap ratio is around 2.5f, tested with some cases
            const int capRatio = 3;
            int innerPathCostMapCap = chunkExitsContainer.Value.Length * capRatio;

            if (innerPathCostMap.Capacity < innerPathCostMapCap)
                innerPathCostMap.Capacity = innerPathCostMapCap;

            innerPathCostMap.Clear();

            var newLineRequests = new NativeQueue<LineCacheKey>(Allocator.TempJob);

            state.Dependency = new PathCostsBakingJob
            {
                CostMap = costMap,
                CachedLines = cachedLines,
                ChunkExitIndexesContainer = chunkExitIndexesContainer,
                ChunkExitsContainer = chunkExitsContainer,
                ChunkIndexToExitIndexesMap = chunkIndexToExitIndexesMap,
                HighestExitCount = highestExitCount,
                InnerPathCostMap = innerPathCostMap.AsParallelWriter(),
                NewLineRequests = newLineRequests.AsParallelWriter(),
            }.ScheduleParallel(chunkList.Value.Length, 64, state.Dependency);

            state.Dependency = new HandleRequestedLinesJob
            {
                NewLineRequests = newLineRequests,
                CachedLines = cachedLines,
            }.Schedule(state.Dependency);

            state.Dependency = newLineRequests.Dispose(state.Dependency);
        }

        [BurstCompile]
        private struct PathCostsBakingJob : IJobParallelForBatch
        {
            [ReadOnly] public WorldTileCostMap CostMap;
            [ReadOnly] public CachedLines CachedLines;
            [ReadOnly] public ChunkIndexToExitIndexesMap ChunkIndexToExitIndexesMap;
            [ReadOnly] public ChunkExitIndexesContainer ChunkExitIndexesContainer;
            [ReadOnly] public ChunkExitsContainer ChunkExitsContainer;
            [ReadOnly] public int HighestExitCount;

            public NativeParallelHashMap<InnerPathKey, float>.ParallelWriter InnerPathCostMap;
            public NativeQueue<LineCacheKey>.ParallelWriter NewLineRequests;

            [BurstCompile]
            public void Execute(int startIndex, int count)
            {
                int upperBound = startIndex + count;

                PathCostComputer costComputer = new()
                {
                    CostMap = this.CostMap,
                    GlobalCachedLines = this.CachedLines,
                    LocalCachedLines = new()
                    {
                        Value = new(20, Allocator.Temp),
                    },
                };

                var exits = new NativeList<ChunkExit>(this.HighestExitCount, Allocator.Temp);
                exits.AddReplicate(default, this.HighestExitCount);

                for (int chunkIndex = startIndex; chunkIndex < upperBound; chunkIndex++)
                {
                    ChunkExitHelper.GetExitsFromChunk(
                        in this.ChunkIndexToExitIndexesMap
                        , in this.ChunkExitIndexesContainer
                        , in this.ChunkExitsContainer
                        , chunkIndex
                        , ref exits
                        , out int exitCount);

                    this.BakeChunkInnerPathCosts(ref costComputer, chunkIndex, in exits, exitCount);
                }

                this.PushLocalCacheToRequestsQueue(in costComputer);
            }

            [BurstCompile]
            private void BakeChunkInnerPathCosts(
                ref PathCostComputer costComputer
                , int chunkIndex
                , in NativeList<ChunkExit> exits
                , int exitCount)
            {
                for (int j = 0; j < exitCount; j++)
                {
                    ChunkExitHelper.GetUnsafeInnerCellFromExit(in this.CostMap, exits[j], chunkIndex, out _, out int cell0MapIndex);
                    int2 pos0 = WorldMapHelper.MapIndexToGridPos(this.CostMap.Width, in this.CostMap.Offset, cell0MapIndex);

                    for (int k = j + 1; k < exitCount; k++)
                    {
                        ChunkExitHelper.GetUnsafeInnerCellFromExit(in this.CostMap, exits[k], chunkIndex, out _, out int cell1MapIndex);

                        bool canSkipPathWithCostOfZero = cell0MapIndex == cell1MapIndex;
                        if (canSkipPathWithCostOfZero) continue;

                        int2 pos1 = WorldMapHelper.MapIndexToGridPos(this.CostMap.Width, in this.CostMap.Offset, cell1MapIndex);

                        costComputer.Pos0 = pos0;
                        costComputer.Pos1 = pos1;

                        float finalCost = costComputer.GetCost();

                        int firstCellMapIndex = cell0MapIndex;
                        int secondCellMapIndex = cell1MapIndex;

                        this.InnerPathCostMap.TryAdd(new InnerPathKey(firstCellMapIndex, secondCellMapIndex), finalCost);
                    }
                }

            }

            [BurstCompile]
            private void PushLocalCacheToRequestsQueue(in PathCostComputer costComputer)
            {
                var keys = costComputer.LocalCachedLines.Value.GetKeyArray(Allocator.Temp);
                foreach (var key in keys)
                {
                    this.NewLineRequests.Enqueue(key);
                }
            }

        }

        [BurstCompile]
        private struct HandleRequestedLinesJob : IJob
        {
            public NativeQueue<LineCacheKey> NewLineRequests;
            public CachedLines CachedLines;

            [BurstCompile]
            public void Execute()
            {
                while (this.NewLineRequests.TryDequeue(out var key))
                {
                    if (this.CachedLines.Value.ContainsKey(key)) continue;

                    PathCostComputer.CreateBresenhamLine(
                        key.Delta.x
                        , key.Delta.y
                        , Allocator.Temp
                        , out var newLine);

                    int length = newLine.Length;
                    for (int i = 0; i < length; i++)
                    {
                        this.CachedLines.Value.Add(key, newLine[i]);
                    }
                }
            }
        }

    }

}
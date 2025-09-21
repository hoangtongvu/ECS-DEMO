using Components.Misc.WorldMap;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Utilities;
using Components.Misc.WorldMap.LineCaching;
using Components.Misc.WorldMap.ChunkInnerPathCost;
using Core.Misc.WorldMap.ChunkInnerPathCost;
using Utilities.Helpers;
using Utilities.Helpers.Misc.WorldMap.ChunkInnerPathCost;
using Core.Misc.WorldMap;
using Utilities.Helpers.Misc.WorldMap;
using Unity.Jobs;

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

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new CellPosRangeMap
                {
                    Value = new(100, Allocator.Persistent),
                });

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new CellPositionsContainer
                {
                    Value = new(500, Allocator.Persistent),
                });

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new InnerPathCostMap
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
            var cellPosRangeMap = SystemAPI.GetSingleton<CellPosRangeMap>();
            var cellPositionsContainer = SystemAPI.GetSingleton<CellPositionsContainer>();
            var chunkList = SystemAPI.GetSingleton<ChunkList>();
            var chunkIndexToExitIndexesMap = SystemAPI.GetSingleton<ChunkIndexToExitIndexesMap>();
            var chunkExitIndexesContainer = SystemAPI.GetSingleton<ChunkExitIndexesContainer>();
            var chunkExitsContainer = SystemAPI.GetSingleton<ChunkExitsContainer>();
            var innerPathCostMap = SystemAPI.GetSingleton<InnerPathCostMap>();
            var highestExitCount = SystemAPI.GetSingleton<HighestExitCount>().Value;

            // TODO: Find another way to get this value
            // The cap ratio is around 2.5f, tested with some cases
            const int capRatio = 3;
            int innerPathCostMapCap = chunkExitsContainer.Value.Length * capRatio;

            var localPathCostMap = new NativeParallelHashMap<InnerPathKey, float>(innerPathCostMapCap, Allocator.TempJob);
            innerPathCostMap.Value.Clear();

            state.Dependency = new PathCostsBakingJob
            {
                CostMap = costMap,
                CellPosRangeMap = cellPosRangeMap,
                CellPositionsContainer = cellPositionsContainer,
                ChunkExitIndexesContainer = chunkExitIndexesContainer,
                ChunkExitsContainer = chunkExitsContainer,
                ChunkIndexToExitIndexesMap = chunkIndexToExitIndexesMap,
                HighestExitCount = highestExitCount,
                LocalPathCostMap = localPathCostMap.AsParallelWriter(),
            }.ScheduleParallel(chunkList.Value.Length, 64, state.Dependency);

            state.Dependency = new SyncLocalToGlobalInnerPathCostMap
            {
                LocalPathCostMap = localPathCostMap,
                InnerPathCostMap = innerPathCostMap,
            }.Schedule(state.Dependency);

            state.Dependency = localPathCostMap.Dispose(state.Dependency);
        }

        [BurstCompile]
        private struct PathCostsBakingJob : IJobParallelForBatch
        {
            [ReadOnly] public WorldTileCostMap CostMap;
            [ReadOnly] public ChunkIndexToExitIndexesMap ChunkIndexToExitIndexesMap;
            [ReadOnly] public ChunkExitIndexesContainer ChunkExitIndexesContainer;
            [ReadOnly] public ChunkExitsContainer ChunkExitsContainer;
            [ReadOnly] public int HighestExitCount;

            [ReadOnly] public CellPosRangeMap CellPosRangeMap;
            [ReadOnly] public CellPositionsContainer CellPositionsContainer;

            public NativeParallelHashMap<InnerPathKey, float>.ParallelWriter LocalPathCostMap;

            [BurstCompile]
            public void Execute(int startIndex, int count)
            {
                int upperBound = startIndex + count;

                PathCostComputer costComputer = new()
                {
                    CostMap = this.CostMap,
                    InputCellPosRangeMap = this.CellPosRangeMap,
                    InputCellPositionsContainer = this.CellPositionsContainer,
                    LocalCachedLines = new(20, Allocator.Temp),
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

                        this.LocalPathCostMap.TryAdd(new InnerPathKey(firstCellMapIndex, secondCellMapIndex), finalCost);
                    }
                }

            }

        }

        [BurstCompile]
        private struct SyncLocalToGlobalInnerPathCostMap : IJob
        {
            [ReadOnly] public NativeParallelHashMap<InnerPathKey, float> LocalPathCostMap;
            [WriteOnly] public InnerPathCostMap InnerPathCostMap;

            [BurstCompile]
            public void Execute()
            {
                foreach (var keyValue in this.LocalPathCostMap)
                {
                    this.InnerPathCostMap.Value.TryAdd(keyValue.Key, keyValue.Value);
                }
            }
        }

    }

}
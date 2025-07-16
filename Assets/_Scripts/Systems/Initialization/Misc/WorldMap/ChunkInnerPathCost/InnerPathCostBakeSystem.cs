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

            innerPathCostMap.Value.Clear();

            FinalCostComputer finalCostComputer = new()
            {
                CostMap = costMap,
                CellPosRangeMap = cellPosRangeMap,
                CellPositionsContainer = cellPositionsContainer,
            };

            var exits = new NativeList<ChunkExit>(highestExitCount, Allocator.Temp);
            exits.AddReplicate(default, highestExitCount);

            int chunkListLength = chunkList.Value.Length;
            for (int chunkIndex = 0; chunkIndex < chunkListLength; chunkIndex++)
            {
                ChunkExitHelper.GetExitsFromChunk(
                    in chunkIndexToExitIndexesMap
                    , in chunkExitIndexesContainer
                    , in chunkExitsContainer
                    , chunkIndex
                    , ref exits
                    , out int exitCount);

                this.BakeChunkInnerPathCosts(in costMap, in innerPathCostMap, ref finalCostComputer, chunkIndex, in exits, exitCount);

            }

        }

        [BurstCompile]
        private void BakeChunkInnerPathCosts(
            in WorldTileCostMap costMap
            , in InnerPathCostMap innerPathCostMap
            , ref FinalCostComputer finalCostComputer
            , int chunkIndex
            , in NativeList<ChunkExit> exits
            , int exitCount)
        {
            for (int j = 0; j < exitCount; j++)
            {
                ChunkExitHelper.GetUnsafeInnerCellFromExit(in costMap, exits[j], chunkIndex, out _, out int cell0MapIndex);
                int2 pos0 = WorldMapHelper.MapIndexToGridPos(costMap.Width, in costMap.Offset, cell0MapIndex);

                for (int k = j + 1; k < exitCount; k++)
                {
                    ChunkExitHelper.GetUnsafeInnerCellFromExit(in costMap, exits[k], chunkIndex, out _, out int cell1MapIndex);

                    bool canSkipPathWithCostOfZero = cell0MapIndex == cell1MapIndex;
                    if (canSkipPathWithCostOfZero) continue;

                    int2 pos1 = WorldMapHelper.MapIndexToGridPos(costMap.Width, in costMap.Offset, cell1MapIndex);

                    finalCostComputer.Pos0 = pos0;
                    finalCostComputer.Pos1 = pos1;

                    float finalCost = finalCostComputer.GetCost();

                    int firstCellMapIndex = cell0MapIndex;
                    int secondCellMapIndex = cell1MapIndex;

                    innerPathCostMap.Value.TryAdd(new InnerPathKey(firstCellMapIndex, secondCellMapIndex), finalCost);

                }

            }

        }

    }

}
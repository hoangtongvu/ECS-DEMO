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

namespace Systems.Initialization.Misc.WorldMap.ChunkInnerPathCost
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(TestMapInitSystem))]
    [UpdateAfter(typeof(CreateChunkExitsSystem))]
    [BurstCompile]
    public partial struct ChunkInnerPathCostBakeSystem : ISystem
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
            var chunkIndexToExitsMap = SystemAPI.GetSingleton<ChunkIndexToExitsMap>();
            var chunkExitsContainer = SystemAPI.GetSingleton<ChunkExitsContainer>();
            var innerPathCostMap = SystemAPI.GetSingleton<InnerPathCostMap>();

            FinalCostComputer finalCostComputer = new()
            {
                CostMap = costMap,
                CellPosRangeMap = cellPosRangeMap,
                CellPositionsContainer = cellPositionsContainer,
            };

            int chunkListLength = chunkList.Value.Length;
            for (int i = 0; i < chunkListLength; i++)
            {
                var chunkExitRange = chunkIndexToExitsMap.Value[i];
                var exits = this.GetExitsFromChunk(in chunkExitsContainer, in chunkExitRange, Allocator.Temp);

                this.BakeChunkInnerPathCosts(in costMap, in innerPathCostMap, ref finalCostComputer, in exits, chunkExitRange.Amount);

                exits.Dispose();

            }

        }

        [BurstCompile]
        private NativeArray<ChunkExit> GetExitsFromChunk(
            in ChunkExitsContainer chunkExitsContainer
            , in ChunkExitRange chunkExitRange
            , Allocator allocator)
        {
            var exits = new NativeArray<ChunkExit>(chunkExitRange.Amount, allocator);
            int upperBound = chunkExitRange.StartIndex + chunkExitRange.Amount;
            int tempIndex = 0;

            for (int j = chunkExitRange.StartIndex; j < upperBound; j++)
            {
                exits[tempIndex] = chunkExitsContainer.Value[j];
                tempIndex++;
            }

            return exits;

        }

        [BurstCompile]
        private void BakeChunkInnerPathCosts(
            in WorldTileCostMap costMap
            , in InnerPathCostMap innerPathCostMap
            , ref FinalCostComputer finalCostComputer
            , in NativeArray<ChunkExit> exits
            , int exitCount)
        {
            for (int j = 0; j < exitCount; j++)
            {
                int2 pos0 = exits[j].InnerCellPos;

                for (int k = j + 1; k < exitCount; k++)
                {
                    int2 pos1 = exits[k].InnerCellPos;

                    finalCostComputer.Pos0 = pos0;
                    finalCostComputer.Pos1 = pos1;

                    float finalCost = finalCostComputer.GetCost();

                    // Store cost to chunk
                    bool canAdd = innerPathCostMap.Value.TryAdd(new InnerPathKey
                    {
                        FirstCellMapIndex = WorldMapHelper.GridPosToMapIndex(costMap.Width, in costMap.Offset, in pos0),
                        SecondCellMapIndex = WorldMapHelper.GridPosToMapIndex(costMap.Width, in costMap.Offset, in pos1),
                    }, finalCost);

                    if (!canAdd) continue;
                    UnityEngine.Debug.Log($"{pos0}, {pos1} - finalCost: {finalCost}");

                }

            }

        }

    }

}
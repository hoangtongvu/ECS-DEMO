using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.LineCaching;
using Components.Misc.WorldMap.ChunkInnerPathCost;
using Components.Misc;

namespace Systems.Simulation.Misc.WorldMap.PathFinding
{
    [UpdateInGroup(typeof(PathFindingSystemGroup))]
    [BurstCompile]
    public partial struct HPAPathFindingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldTileCostMap>();
            state.RequireForUpdate<CellPosRangeMap>();
            state.RequireForUpdate<CellPositionsContainer>();
            state.RequireForUpdate<ChunkIndexToExitIndexesMap>();
            state.RequireForUpdate<ChunkExitIndexesContainer>();
            state.RequireForUpdate<ChunkExitsContainer>();
            state.RequireForUpdate<InnerPathCostMap>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var costMap = SystemAPI.GetSingleton<WorldTileCostMap>();
            var cellPosRangeMap = SystemAPI.GetSingleton<CellPosRangeMap>();
            var cellPositionsContainer = SystemAPI.GetSingleton<CellPositionsContainer>();
            var chunkIndexToExitIndexesMap = SystemAPI.GetSingleton<ChunkIndexToExitIndexesMap>();
            var exitIndexesContainer = SystemAPI.GetSingleton<ChunkExitIndexesContainer>();
            var exitsContainer = SystemAPI.GetSingleton<ChunkExitsContainer>();
            var innerPathCostMap = SystemAPI.GetSingleton<InnerPathCostMap>();
            half cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;

            state.Dependency = new HPAPathFindingJob
            {
                CostMap = costMap,
                CellPosRangeMap = cellPosRangeMap,
                CellPositionsContainer = cellPositionsContainer,
                ChunkIndexToExitIndexesMap = chunkIndexToExitIndexesMap,
                ChunkExitIndexesContainer = exitIndexesContainer,
                ChunkExitsContainer = exitsContainer,
                InnerPathCostMap = innerPathCostMap,
                CellRadius = cellRadius,
            }.ScheduleParallel(state.Dependency);
        }

    }

}
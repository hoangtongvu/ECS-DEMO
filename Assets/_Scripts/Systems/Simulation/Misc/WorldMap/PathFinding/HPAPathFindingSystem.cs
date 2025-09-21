using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.LineCaching;
using Components.Misc.WorldMap.ChunkInnerPathCost;

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
            state.RequireForUpdate<CachedLines>();
            state.RequireForUpdate<ChunkIndexToExitIndexesMap>();
            state.RequireForUpdate<ChunkExitIndexesContainer>();
            state.RequireForUpdate<ChunkExitsContainer>();
            state.RequireForUpdate<InnerPathCostMap>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var costMap = SystemAPI.GetSingleton<WorldTileCostMap>();
            var cachedLines = SystemAPI.GetSingleton<CachedLines>();
            var chunkIndexToExitIndexesMap = SystemAPI.GetSingleton<ChunkIndexToExitIndexesMap>();
            var exitIndexesContainer = SystemAPI.GetSingleton<ChunkExitIndexesContainer>();
            var exitsContainer = SystemAPI.GetSingleton<ChunkExitsContainer>();
            var innerPathCostMap = SystemAPI.GetSingleton<InnerPathCostMap>();
            half cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;

            state.Dependency = new HPAPathFindingJob
            {
                CostMap = costMap,
                CachedLines = cachedLines,
                ChunkIndexToExitIndexesMap = chunkIndexToExitIndexesMap,
                ChunkExitIndexesContainer = exitIndexesContainer,
                ChunkExitsContainer = exitsContainer,
                InnerPathCostMap = innerPathCostMap,
                CellRadius = cellRadius,
            }.ScheduleParallel(state.Dependency);
        }

    }

}
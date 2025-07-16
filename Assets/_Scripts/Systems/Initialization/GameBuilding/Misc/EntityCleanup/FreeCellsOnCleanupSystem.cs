using Components.GameEntity.Misc;
using Components.GameEntity.Misc.EntityCleanup;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.Misc;
using Systems.Initialization.GameEntity.Misc.EntityCleanup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Utilities.Extensions;

namespace Systems.Initialization.GameBuilding.Misc.EntityCleanup
{
    [UpdateInGroup(typeof(CleanupEntityHandleSystemGroup))]
    [BurstCompile]
    public partial struct FreeCellsOnCleanupSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedCleanupEntityTag
                    , GameEntitySizeHolder
                    , TopLeftCellPos>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;

            var gameEntitySizeHolders = this.query.ToComponentDataArray<GameEntitySizeHolder>(Allocator.Temp);
            var topLeftCellPositions = this.query.ToComponentDataArray<TopLeftCellPos>(Allocator.Temp);

            var worldTileCostMap = SystemAPI.GetSingleton<WorldTileCostMap>();

            for (int i = 0; i < length; i++)
            {
                var buildingEntity = entities[i];
                var gameEntitySize = gameEntitySizeHolders[i].Value;
                var topLeftCellPos = topLeftCellPositions[i].Value;

                this.FreeCells(ref worldTileCostMap, in topLeftCellPos, in gameEntitySize.GridSquareSize);
            }

            SystemAPI.GetSingletonRW<WorldMapChangedTag>().ValueRW.Value = true;

        }

        [BurstCompile]
        private void FreeCells(
            ref WorldTileCostMap worldTileCostMap
            , in int2 topLeftCellPos
            , in int gridSquareSize)
        {
            const byte emptyTileCost = 1;

            int xBound = topLeftCellPos.x + gridSquareSize;
            int yBound = topLeftCellPos.y + gridSquareSize;

            for (int x = topLeftCellPos.x; x < xBound; x++)
            {
                for (int y = topLeftCellPos.y; y < yBound; y++)
                {
                    ref var cell = ref worldTileCostMap.GetRefCellAt(x, y);
                    cell.Cost = emptyTileCost;
                }
            }

        }

    }

}
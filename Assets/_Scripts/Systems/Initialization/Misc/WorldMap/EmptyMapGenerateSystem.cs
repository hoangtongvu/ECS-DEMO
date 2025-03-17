using Components.Misc.WorldMap;
using Core.Misc.WorldMap;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Utilities;
using Utilities.Helpers;

namespace Systems.Initialization.Misc.WorldMap
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(WorldMapChangedTagClearSystem))]
    public partial class EmptyMapGenerateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<EmptyMapTag>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            this.CreateWorldMap();
        }

        private void CreateWorldMap()
        {
            int mapWidth = 160;
            int mapHeight = 90;

            var costMap = new NativeArray<Cell>(mapWidth * mapHeight, Allocator.Persistent);

            // Parse CSV and fill in the FlowFieldGridNode array
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    const byte cost = 1;
                    int arrayIndex = (y * mapWidth) + x;

                    costMap[arrayIndex] = new()
                    {
                        Cost = cost,
                        ChunkIndex = -1,
                    };
                }
            }

            half cellRadius = new(0.7f);
            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new CellRadius
                {
                    Value = cellRadius,
                });

            WorldMapHelper.GetGridOffset(mapWidth, mapHeight, out var gridOffset);

            //this.CreateMapSizeComponents(mapWidth, mapHeight);
            this.CreateMapOffsetComponent(in gridOffset);
            this.CreateCostMap(in costMap, mapWidth, mapHeight, in gridOffset);

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new WorldMapChangedTag
                {
                    Value = true,
                });

        }

        private void CreateMapOffsetComponent(in int2 gridOffset)
        {
            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new MapGridOffset
                {
                    Value = gridOffset,
                });

        }

        private void CreateCostMap(in NativeArray<Cell> costs, int mapWidth, int mapHeight, in int2 offset)
        {
            var costMap = new WorldTileCostMap
            {
                Value = costs,
                Width = mapWidth,
                Height = mapHeight,
                Offset = offset,
            };

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(costMap);

        }

    }

}
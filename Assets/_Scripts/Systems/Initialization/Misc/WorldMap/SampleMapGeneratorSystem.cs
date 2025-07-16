using Components.GameEntity;
using Components.Harvest;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.WorldBuilding;
using Components.Player;
using Core.GameEntity;
using Core.Harvest;
using Core.Misc.WorldMap;
using Core.Misc.WorldMap.WorldBuilding;
using Core.Utilities.Extensions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Utilities;
using Utilities.Helpers;

namespace Systems.Initialization.Misc.WorldMap
{
    // TODO: Can make this ISystem??
    [UpdateInGroup(typeof(MapGenerateSystemGroup))]
    public partial class SampleMapGeneratorSystem : SystemBase
    {
        private EntityQuery playerQuery;
        private Random rand;

        protected override void OnCreate()
        {
            this.playerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerProfilesSOHolder
                    , BakedGameEntityProfileElement>()
                .Build();

            this.RequireForUpdate(this.playerQuery);

            this.rand = new Random(47);

            this.RequireForUpdate<SampleMapTag>();
            this.RequireForUpdate<GameBuildingPrefabEntityMap>();
            this.RequireForUpdate<HarvesteePrefabEntityMap>();
            this.RequireForUpdate<GameEntitySizeMap>();
            this.RequireForUpdate<BuildCommandList>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var gameBuildingPrefabEntityMap = SystemAPI.GetSingleton<GameBuildingPrefabEntityMap>().Value;
            var harvesteePrefabEntityMap = SystemAPI.GetSingleton<HarvesteePrefabEntityMap>().Value;
            var gameEntitySizeMap = SystemAPI.GetSingleton<GameEntitySizeMap>().Value;
            var buildCommandList = SystemAPI.GetSingleton<BuildCommandList>();

            this.CreateWorldMap(in physicsWorld, in gameBuildingPrefabEntityMap, in harvesteePrefabEntityMap, in gameEntitySizeMap, in buildCommandList);

            var bakedProfileElements = this.playerQuery.GetSingletonBuffer<BakedGameEntityProfileElement>();

            this.EntityManager.Instantiate(bakedProfileElements[0].PrimaryEntity);
        }

        private void CreateWorldMap(
            in PhysicsWorldSingleton physicsWorld
            , in NativeHashMap<GameBuildingProfileId, Entity> gameBuildingPrefabEntityMap
            , in NativeHashMap<HarvesteeProfileId, Entity> harvesteePrefabEntityMap
            , in NativeHashMap<Entity, GameEntitySize> gameEntitySizeMap
            , in BuildCommandList buildCommandList)
        {
            const int mapWidth = 160;
            const int mapHeight = 90;
            half cellRadius = new(1f);
            WorldMapHelper.GetGridOffset(mapWidth, mapHeight, out int2 gridOffset);

            var su = SingletonUtilities.GetInstance(this.EntityManager);
            
            var costMap = new NativeArray<Cell>(mapWidth * mapHeight, Allocator.Persistent);

            this.GenerateMap(
                in physicsWorld
                , in gameBuildingPrefabEntityMap
                , in harvesteePrefabEntityMap
                , in gameEntitySizeMap
                , ref costMap
                , in buildCommandList
                , mapWidth, mapHeight
                , in gridOffset
                , in cellRadius);

            this.CreateMapOffsetComponent(in gridOffset);
            this.CreateCostMap(in costMap, mapWidth, mapHeight, in gridOffset);

            su.AddOrSetComponentData(new CellRadius
            {
                Value = cellRadius,
            });

            su.AddOrSetComponentData(new WorldMapChangedTag
            {
                Value = true,
            });

        }

        private void GenerateMap(
            in PhysicsWorldSingleton physicsWorld
            , in NativeHashMap<GameBuildingProfileId, Entity> gameBuildingPrefabEntityMap
            , in NativeHashMap<HarvesteeProfileId, Entity> harvesteePrefabEntityMap
            , in NativeHashMap<Entity, GameEntitySize> gameEntitySizeMap
            , ref NativeArray<Cell> costMap
            , in BuildCommandList buildCommandList
            , int mapWidth
            , int mapHeight
            , in int2 gridOffset
            , in half cellRadius)
        {
            this.PlaceEmptyTilesOnMap(ref costMap, mapWidth, mapHeight);
            this.PlaceRandomHousesOnMap(in physicsWorld, in gameBuildingPrefabEntityMap, in gameEntitySizeMap, ref costMap, in buildCommandList, mapWidth, mapHeight, in gridOffset, in cellRadius);
            this.PlaceRandomHarvesteesOnMap(in physicsWorld, in harvesteePrefabEntityMap, in gameEntitySizeMap, ref costMap, in buildCommandList, mapWidth, mapHeight, in gridOffset, in cellRadius);
        }

        private void PlaceEmptyTilesOnMap(ref NativeArray<Cell> costMap, int mapWidth, int mapHeight)
        {
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

        }

        private void PlaceRandomHousesOnMap(
            in PhysicsWorldSingleton physicsWorld
            , in NativeHashMap<GameBuildingProfileId, Entity> gameBuildingPrefabEntityMap
            , in NativeHashMap<Entity, GameEntitySize> gameEntitySizeMap
            , ref NativeArray<Cell> costMap
            , in BuildCommandList buildCommandList
            , int mapWidth
            , int mapHeight
            , in int2 gridOffset
            , in half cellRadius)
        {
            const float placementPercentage = 0.005f;

            var houseId = new GameBuildingProfileId
            {
                VariantIndex = 0,
            };

            var entityPrefab = gameBuildingPrefabEntityMap[houseId];

            this.PlaceRandomEntitiesOnMap(
                in physicsWorld
                , in gameEntitySizeMap
                , ref costMap
                , in buildCommandList
                , mapWidth, mapHeight
                , in gridOffset, in cellRadius
                , placementPercentage
                , in entityPrefab);

        }

        private void PlaceRandomHarvesteesOnMap(
            in PhysicsWorldSingleton physicsWorld
            , in NativeHashMap<HarvesteeProfileId, Entity> harvesteePrefabEntityMap
            , in NativeHashMap<Entity, GameEntitySize> gameEntitySizeMap
            , ref NativeArray<Cell> costMap
            , in BuildCommandList buildCommandList
            , int mapWidth
            , int mapHeight
            , in int2 gridOffset
            , in half cellRadius)
        {
            this.PlaceRandomTreesOnMap(
                in physicsWorld
                , in harvesteePrefabEntityMap
                , in gameEntitySizeMap
                , ref costMap
                , in buildCommandList
                , mapWidth, mapHeight
                , in gridOffset, in cellRadius);

            this.PlaceRandomRocksOnMap(
                in physicsWorld
                , in harvesteePrefabEntityMap
                , in gameEntitySizeMap
                , ref costMap
                , in buildCommandList
                , mapWidth, mapHeight
                , in gridOffset, in cellRadius);

            this.PlaceRandomBerryBushessOnMap(
                in physicsWorld
                , in harvesteePrefabEntityMap
                , in gameEntitySizeMap
                , ref costMap
                , in buildCommandList
                , mapWidth, mapHeight
                , in gridOffset, in cellRadius);

        }

        private void PlaceRandomTreesOnMap(
            in PhysicsWorldSingleton physicsWorld
            , in NativeHashMap<HarvesteeProfileId, Entity> harvesteePrefabEntityMap
            , in NativeHashMap<Entity, GameEntitySize> gameEntitySizeMap
            , ref NativeArray<Cell> costMap
            , in BuildCommandList buildCommandList
            , int mapWidth
            , int mapHeight
            , in int2 gridOffset
            , in half cellRadius)
        {
            const float placementPercentage = 0.02f;

            var treeId = new HarvesteeProfileId
            {
                HarvesteeType = HarvesteeType.Tree,
                VariantIndex = 0,
            };

            var entityPrefab = harvesteePrefabEntityMap[treeId];

            this.PlaceRandomEntitiesOnMap(
                in physicsWorld
                , in gameEntitySizeMap
                , ref costMap
                , in buildCommandList
                , mapWidth, mapHeight
                , in gridOffset, in cellRadius
                , placementPercentage
                , in entityPrefab);

        }

        private void PlaceRandomRocksOnMap(
            in PhysicsWorldSingleton physicsWorld
            , in NativeHashMap<HarvesteeProfileId, Entity> harvesteePrefabEntityMap
            , in NativeHashMap<Entity, GameEntitySize> gameEntitySizeMap
            , ref NativeArray<Cell> costMap
            , in BuildCommandList buildCommandList
            , int mapWidth
            , int mapHeight
            , in int2 gridOffset
            , in half cellRadius)
        {
            const float placementPercentage = 0.01f;

            var treeId = new HarvesteeProfileId
            {
                HarvesteeType = HarvesteeType.ResourcePit,
                VariantIndex = 0,
            };

            var entityPrefab = harvesteePrefabEntityMap[treeId];

            this.PlaceRandomEntitiesOnMap(
                in physicsWorld
                , in gameEntitySizeMap
                , ref costMap
                , in buildCommandList
                , mapWidth, mapHeight
                , in gridOffset, in cellRadius
                , placementPercentage
                , in entityPrefab);

        }

        private void PlaceRandomBerryBushessOnMap(
            in PhysicsWorldSingleton physicsWorld
            , in NativeHashMap<HarvesteeProfileId, Entity> harvesteePrefabEntityMap
            , in NativeHashMap<Entity, GameEntitySize> gameEntitySizeMap
            , ref NativeArray<Cell> costMap
            , in BuildCommandList buildCommandList
            , int mapWidth
            , int mapHeight
            , in int2 gridOffset
            , in half cellRadius)
        {
            const float placementPercentage = 0.01f;

            var prefabId = new HarvesteeProfileId
            {
                HarvesteeType = HarvesteeType.BerryBush,
                VariantIndex = 0,
            };

            var entityPrefab = harvesteePrefabEntityMap[prefabId];

            this.PlaceRandomEntitiesOnMap(
                in physicsWorld
                , in gameEntitySizeMap
                , ref costMap
                , in buildCommandList
                , mapWidth, mapHeight
                , in gridOffset, in cellRadius
                , placementPercentage
                , in entityPrefab);

        }

        private void PlaceRandomEntitiesOnMap(
            in PhysicsWorldSingleton physicsWorld
            , in NativeHashMap<Entity, GameEntitySize> gameEntitySizeMap
            , ref NativeArray<Cell> costMap
            , in BuildCommandList buildCommandList
            , int mapWidth
            , int mapHeight
            , in int2 gridOffset
            , in half cellRadius
            , in float placementPercentage
            , in Entity entityPrefab)
        {
            var entitySize = gameEntitySizeMap[entityPrefab];

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    bool cellsArePassable = this.CellsArePassable(in costMap, x, y, mapWidth, mapHeight, entitySize.GridSquareSize);
                    if (!cellsArePassable) continue;

                    float randomValue = this.rand.NextFloat(0, 100);
                    if (randomValue > placementPercentage * 100) continue;

                    this.MarkCellsAsObstacles(in costMap, x, y, mapWidth, entitySize.GridSquareSize);
                    this.SpawnEntity(
                        in buildCommandList
                        , in entityPrefab
                        , in entitySize
                        , in gridOffset
                        , x, y);

                }
            }

        }

        private bool CellsArePassable(
            in NativeArray<Cell> costMap
            , int topLeftX
            , int topLeftY
            , int mapWidth
            , int mapHeight
            , int gridSquareSize)
        {
            int upperBoundY = topLeftY + gridSquareSize;
            int upperBoundX = topLeftX + gridSquareSize;

            for (int y = topLeftY; y < upperBoundY; y++)
            {
                for (int x = topLeftX; x < upperBoundX; x++)
                {
                    if (x >= mapWidth || y >= mapHeight) return false;

                    int arrayIndex = (y * mapWidth) + x;
                    var cell = costMap[arrayIndex];

                    if (cell.IsPassable()) continue;
                    return false;
                }
            }

            return true;

        }

        private void MarkCellsAsObstacles(
            in NativeArray<Cell> costMap
            , int topLeftX
            , int topLeftY
            , int mapWidth
            , int gridSquareSize)
        {
            int upperBoundY = topLeftY + gridSquareSize;
            int upperBoundX = topLeftX + gridSquareSize;

            for (int y = topLeftY; y < upperBoundY; y++)
            {
                for (int x = topLeftX; x < upperBoundX; x++)
                {
                    int arrayIndex = (y * mapWidth) + x;
                    ref var cell = ref costMap.ElementAt(arrayIndex);
                    cell.Cost = byte.MaxValue;
                }
            }

        }

        private void SpawnEntity(
            in BuildCommandList buildCommandList
            , in Entity prefabEntity
            , in GameEntitySize gameEntitySize
            , in int2 gridOffset
            , int x
            , int y)
        {
            buildCommandList.Value.Add(new()
            {
                Entity = prefabEntity,
                TopLeftCellGridPos = new(x + gridOffset.x, y + gridOffset.y),
                GameEntitySize = gameEntitySize,
                SpawnerEntity = Entity.Null,
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
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
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Utilities;
using Utilities.Helpers;

namespace Systems.Initialization.Misc.WorldMap
{
    [UpdateInGroup(typeof(MapGenerateSystemGroup))]
    [BurstCompile]
    public partial struct SampleMapGeneratorSystem : ISystem
    {
        private EntityQuery playerQuery;
        private Random rand;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.playerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerProfilesSOHolder
                    , BakedGameEntityProfileElement>()
                .Build();

            state.RequireForUpdate(this.playerQuery);

            this.rand = new Random(47);

            state.RequireForUpdate<SampleMapTag>();
            state.RequireForUpdate<GameBuildingPrefabEntityMap>();
            state.RequireForUpdate<HarvesteePrefabEntityMap>();
            state.RequireForUpdate<GameEntitySizeMap>();
            state.RequireForUpdate<BuildCommandList>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var gameBuildingPrefabEntityMap = SystemAPI.GetSingleton<GameBuildingPrefabEntityMap>().Value;
            var harvesteePrefabEntityMap = SystemAPI.GetSingleton<HarvesteePrefabEntityMap>().Value;
            var gameEntitySizeMap = SystemAPI.GetSingleton<GameEntitySizeMap>().Value;
            var buildCommandList = SystemAPI.GetSingleton<BuildCommandList>();

            this.CreateWorldMap(
                ref state
                , in physicsWorld
                , in gameBuildingPrefabEntityMap
                , in harvesteePrefabEntityMap
                , in gameEntitySizeMap
                , in buildCommandList);

            var bakedProfileElements = this.playerQuery.GetSingletonBuffer<BakedGameEntityProfileElement>();

            state.EntityManager.Instantiate(bakedProfileElements[0].PrimaryEntity);
        }

        [BurstCompile]
        private void CreateWorldMap(
            ref SystemState state
            , in PhysicsWorldSingleton physicsWorld
            , in NativeHashMap<GameBuildingProfileId, Entity> gameBuildingPrefabEntityMap
            , in NativeHashMap<HarvesteeProfileId, Entity> harvesteePrefabEntityMap
            , in NativeHashMap<Entity, GameEntitySize> gameEntitySizeMap
            , in BuildCommandList buildCommandList)
        {
            const int mapWidth = 160;
            const int mapHeight = 160;
            half cellRadius = new(1f);
            WorldMapHelper.GetGridOffset(mapWidth, mapHeight, out int2 gridOffset);

            var su = SingletonUtilities.GetInstance(state.EntityManager);
            
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

            this.CreateMapOffsetComponent(ref state, in gridOffset);
            this.CreateCostMap(ref state, in costMap, mapWidth, mapHeight, in gridOffset);

            su.AddOrSetComponentData(new CellRadius
            {
                Value = cellRadius,
            });

            su.AddOrSetComponentData(new WorldMapChangedTag
            {
                Value = true,
            });

        }

        [BurstCompile]
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

        [BurstCompile]
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

        [BurstCompile]
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
            const float placementPercentage = 0.0005f;

            const int prefabCount = 1;
            var prefabs = new NativeArray<Entity>(prefabCount, Allocator.Temp);

            for (int i = 0; i < prefabCount; i++)
            {
                var buildingId = new GameBuildingProfileId
                {
                    VariantIndex = 0,
                };

                var entityPrefab = gameBuildingPrefabEntityMap[buildingId];
                prefabs[i] = entityPrefab;
            }

            this.PlaceRandomEntitiesOnMap(
                in physicsWorld
                , in gameEntitySizeMap
                , ref costMap
                , in buildCommandList
                , mapWidth, mapHeight
                , in gridOffset, in cellRadius
                , placementPercentage
                , in prefabs);

        }

        [BurstCompile]
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

        [BurstCompile]
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

            const int prefabCount = 2;
            var prefabs = new NativeArray<Entity>(prefabCount, Allocator.Temp);

            for (int i = 0; i < prefabCount; i++)
            {
                var harvesteeId = new HarvesteeProfileId
                {
                    HarvesteeType = HarvesteeType.Tree,
                    VariantIndex = (byte)i,
                };

                var entityPrefab = harvesteePrefabEntityMap[harvesteeId];
                prefabs[i] = entityPrefab;
            }

            this.PlaceRandomEntitiesOnMap(
                in physicsWorld
                , in gameEntitySizeMap
                , ref costMap
                , in buildCommandList
                , mapWidth, mapHeight
                , in gridOffset, in cellRadius
                , placementPercentage
                , in prefabs);

        }

        [BurstCompile]
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

            const int prefabCount = 2;
            var prefabs = new NativeArray<Entity>(prefabCount, Allocator.Temp);

            for (int i = 0; i < prefabCount; i++)
            {
                var harvesteeId = new HarvesteeProfileId
                {
                    HarvesteeType = HarvesteeType.ResourcePit,
                    VariantIndex = (byte)i,
                };

                var entityPrefab = harvesteePrefabEntityMap[harvesteeId];
                prefabs[i] = entityPrefab;
            }

            this.PlaceRandomEntitiesOnMap(
                in physicsWorld
                , in gameEntitySizeMap
                , ref costMap
                , in buildCommandList
                , mapWidth, mapHeight
                , in gridOffset, in cellRadius
                , placementPercentage
                , in prefabs);

        }

        [BurstCompile]
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

            const int prefabCount = 1;
            var prefabs = new NativeArray<Entity>(prefabCount, Allocator.Temp);

            for (int i = 0; i < prefabCount; i++)
            {
                var harvesteeId = new HarvesteeProfileId
                {
                    HarvesteeType = HarvesteeType.BerryBush,
                    VariantIndex = (byte)i,
                };

                var entityPrefab = harvesteePrefabEntityMap[harvesteeId];
                prefabs[i] = entityPrefab;
            }

            this.PlaceRandomEntitiesOnMap(
                in physicsWorld
                , in gameEntitySizeMap
                , ref costMap
                , in buildCommandList
                , mapWidth, mapHeight
                , in gridOffset, in cellRadius
                , placementPercentage
                , in prefabs);

        }

        [BurstCompile]
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
            , in NativeArray<Entity> entityPrefabs)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float randomValue = this.rand.NextFloat(0, 100);
                    if (randomValue > placementPercentage * 100) continue;

                    var entityPrefab = entityPrefabs[this.rand.NextInt(0, entityPrefabs.Length)];
                    var entitySize = gameEntitySizeMap[entityPrefab];

                    bool cellsArePassable = this.CellsArePassable(in costMap, x, y, mapWidth, mapHeight, entitySize.GridSquareSize);
                    if (!cellsArePassable) continue;

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

        [BurstCompile]
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

        [BurstCompile]
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

        [BurstCompile]
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

        [BurstCompile]
        private void CreateMapOffsetComponent(ref SystemState state, in int2 gridOffset)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new MapGridOffset
                {
                    Value = gridOffset,
                });

        }

        [BurstCompile]
        private void CreateCostMap(ref SystemState state, in NativeArray<Cell> costs, int mapWidth, int mapHeight, in int2 offset)
        {
            var costMap = new WorldTileCostMap
            {
                Value = costs,
                Width = mapWidth,
                Height = mapHeight,
                Offset = offset,
            };

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(costMap);

        }

    }

}
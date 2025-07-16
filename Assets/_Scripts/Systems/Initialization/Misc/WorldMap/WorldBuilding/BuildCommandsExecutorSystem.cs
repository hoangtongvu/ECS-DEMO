using Components.GameEntity.EntitySpawning;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.Misc;
using Components.Misc.WorldMap.WorldBuilding;
using Core.Misc;
using Core.Misc.WorldMap;
using Core.Utilities.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Utilities;
using Utilities.Extensions;
using Utilities.Helpers;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(MapChangedSystemGroup))]
    [BurstCompile]
    public partial struct BuildCommandsExecutorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new BuildCommandList
                {
                    Value = new(30, Allocator.Persistent),
                });

            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<WorldTileCostMap>();
            state.RequireForUpdate<CellRadius>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandList = SystemAPI.GetSingleton<BuildCommandList>();
            int length = commandList.Value.Length;

            if (length == 0) return;

            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var costMap = SystemAPI.GetSingleton<WorldTileCostMap>();
            var cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;

            SystemAPI.GetSingletonRW<WorldMapChangedTag>().ValueRW.Value = true;
            var em = state.EntityManager;

            for (int i = 0; i < length; i++)
            {
                var buildCommand = commandList.Value[i];
                var newEntity = state.EntityManager.Instantiate(buildCommand.Entity);

                this.GetBuildCenterWorldPos(in physicsWorld, in buildCommand, in cellRadius, out var posOnGround);
                SystemAPI.SetComponent(newEntity, LocalTransform.FromPosition(posOnGround.Add(y: buildCommand.GameEntitySize.ObjectHeight)));

                if (SystemAPI.HasComponent<SpawnerEntityHolder>(newEntity))
                {
                    SystemAPI.SetComponent(newEntity, new SpawnerEntityHolder
                    {
                        Value = buildCommand.SpawnerEntity,
                    });
                }

                em.AddComponentData(newEntity, new TopLeftCellPos
                {
                    Value = buildCommand.TopLeftCellGridPos,
                });

                this.MarkCellsAsObstacle(in costMap, in buildCommand.TopLeftCellGridPos, buildCommand.GameEntitySize.GridSquareSize);

            }

            commandList.Value.Clear();

        }

        [BurstCompile]
        private void GetBuildCenterWorldPos(
            in PhysicsWorldSingleton physicsWorld
            , in BuildCommand buildCommand
            , in half cellRadius
            , out float3 posOnGround)
        {
            posOnGround = default;
            WorldMapHelper.GridPosToWorldPos(in cellRadius, in buildCommand.TopLeftCellGridPos, out float3 topLeftCellWorldPos);

            float3 startPos = topLeftCellWorldPos;
            float addValueXZ = (buildCommand.GameEntitySize.GridSquareSize - 1) * cellRadius;

            startPos.x += addValueXZ;
            startPos.z -= addValueXZ;
            startPos.y = 100f;

            bool hit = this.CastRayToGround(in physicsWorld, in startPos, out var raycastHit);
            if (!hit)
            {
                return;
                // TODO: As the ground in the sample scene is not big enough to cover the whole map width
                // -> this hit will miss some at the both left and right sides -> Can't throw any exception here
                //throw new System.Exception($"Can't hit the Ground with TopLeftCellGridPos: {buildCommand.TopLeftCellGridPos}, GridSquareSize: {buildCommand.GameEntitySize.GridSquareSize}, topLeftCellWorldPos: {topLeftCellWorldPos}, startPos: {startPos}");
            }

            posOnGround = raycastHit.Position;

        }

        [BurstCompile]
        private bool CastRayToGround(
            in PhysicsWorldSingleton physicsWorld
            , in float3 startPos
            , out Unity.Physics.RaycastHit raycastHit)
        {
            float3 rayStart = startPos;
            float3 rayEnd = startPos.Add(y: -500f);

            RaycastInput raycastInput = new()
            {
                Start = rayStart,
                End = rayEnd,
                Filter = new CollisionFilter
                {
                    BelongsTo = (uint)CollisionLayer.Ground,
                    CollidesWith = (uint)CollisionLayer.Ground,
                },
            };

            return physicsWorld.CastRay(raycastInput, out raycastHit);
        }

        [BurstCompile]
        private void MarkCellsAsObstacle(
            in WorldTileCostMap costMap
            , in int2 topLeftGridPos
            , int squareSize)
        {
            for (int y = topLeftGridPos.y; y < topLeftGridPos.y + squareSize; y++)
            {
                for (int x = topLeftGridPos.x; x < topLeftGridPos.x + squareSize; x++)
                {
                    ref Cell cell = ref costMap.GetRefCellAt(x, y);
                    cell.Cost = byte.MaxValue;
                }

            }

        }

    }

}
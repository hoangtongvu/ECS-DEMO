using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Components.Misc.WorldMap.WorldBuilding;
using Components.Misc.WorldMap;
using Utilities;
using Unity.Collections;
using Utilities.Extensions;
using Core.Misc.WorldMap;
using Components.GameEntity.EntitySpawning;

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
                .AddOrSetComponentData(new BuildCommandQueue
                {
                    Value = new(30, Allocator.Persistent),
                });

            state.RequireForUpdate<WorldTileCostMap>();
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandQueue = SystemAPI.GetSingleton<BuildCommandQueue>();
            var costMap = SystemAPI.GetSingleton<WorldTileCostMap>();

            if (commandQueue.Value.Length > 0)
            {
                SystemAPI.GetSingletonRW<WorldMapChangedTag>().ValueRW.Value = true;
            }

            while (this.TryGetCommandFromQueue(in commandQueue, out var buildCommand))
            {
                var newEntity = state.EntityManager.Instantiate(buildCommand.Entity);

                state.EntityManager.SetComponentData(newEntity, LocalTransform.FromPosition(buildCommand.BuildingCenterPos));

                if (SystemAPI.HasComponent<SpawnerEntityHolder>(newEntity))
                {
                    SystemAPI.SetComponent(newEntity, new SpawnerEntityHolder
                    {
                        Value = buildCommand.SpawnerEntity,
                    });
                }

                this.MarkCellsAsObstacle(in costMap, in buildCommand.TopLeftCellGridPos, buildCommand.GridSquareSize);

            }

        }

        [BurstCompile]
        private bool TryGetCommandFromQueue(in BuildCommandQueue buildCommandQueue, out BuildCommand buildCommand)
        {
            if (buildCommandQueue.Value.Length == 0)
            {
                buildCommand = default;
                return false;
            }

            int lastIndex = buildCommandQueue.Value.Length - 1;
            buildCommand = buildCommandQueue.Value[lastIndex];
            buildCommandQueue.Value.RemoveAt(lastIndex);
            return true;

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
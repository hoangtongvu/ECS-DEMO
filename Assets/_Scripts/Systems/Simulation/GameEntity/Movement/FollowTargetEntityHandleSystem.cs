using Components.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.PathFinding;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Utilities.Extensions;
using Utilities.Helpers;

namespace Systems.Simulation.GameEntity.Movement
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class FollowTargetEntityHandleSystemGroup : ComponentSystemGroup
    {
        public FollowTargetEntityHandleSystemGroup()
        {
            this.RateManager = new RateUtils.VariableRateManager(500);
        }
    }

    [UpdateInGroup(typeof(FollowTargetEntityHandleSystemGroup))]
    [BurstCompile]
    public partial struct FollowTargetEntityHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var followerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    MoveCommandElement
                    , WaypointElement
                    , CanMoveEntityTag
                    , CanFindPathTag>()
                .Build();

            var targetQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , MoveableEntityTag>()
                .Build();

            state.RequireForUpdate(followerQuery);
            state.RequireForUpdate(targetQuery);
            state.RequireForUpdate<WorldTileCostMap>();
            state.RequireForUpdate<CellRadius>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var map = SystemAPI.GetSingleton<WorldTileCostMap>();
            var cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;

            foreach (var (moveCommandElementRef, waypoints, entity) in SystemAPI
                .Query<
                    RefRW<MoveCommandElement>
                    , DynamicBuffer<WaypointElement>>()
                .WithAll<
                    CanMoveEntityTag>()
                .WithEntityAccess())
            {
                var targetEntity = moveCommandElementRef.ValueRO.TargetEntity;
                if (!SystemAPI.HasComponent<LocalTransform>(targetEntity)) continue;
                if (!SystemAPI.HasComponent<MoveableEntityTag>(targetEntity)) continue;

                var oldWorldPos = moveCommandElementRef.ValueRO.Float3;
                var newWorldPos = SystemAPI.GetComponent<LocalTransform>(targetEntity).Position;

                WorldMapHelper.WorldPosToGridPos(in cellRadius, in oldWorldPos, out var oldGridPos);
                WorldMapHelper.WorldPosToGridPos(in cellRadius, in newWorldPos, out var newGridPos);

                if (oldGridPos.Equals(newGridPos)) continue;

                moveCommandElementRef.ValueRW.Float3 = newWorldPos;

                map.GetCellAt(in oldGridPos, out var oldCell);
                map.GetCellAt(in newGridPos, out var newCell);

                var oldChunkIndex = oldCell.ChunkIndex;
                var newChunkIndex = newCell.ChunkIndex;

                if (oldChunkIndex == newChunkIndex)
                {
                    if (waypoints.IsEmpty) continue;

                    ref var finalWaypoint = ref waypoints.ElementAt(0);
                    finalWaypoint = new() { Value = newGridPos };

                    continue;
                }

                SystemAPI.SetComponentEnabled<CanFindPathTag>(entity, true);

            }

        }

    }

}
using Components;
using Components.Misc;
using Components.Misc.WorldMap;
using Components.Misc.WorldMap.PathFinding;
using Unity.Burst;
using Unity.Burst.CompilerServices;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utilities.Extensions;
using Utilities.Helpers;

namespace Systems.Simulation.Unit
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct ChangeTargetPosWaypointSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , LocalTransform
                    , DistanceToTarget
                    , TargetPosition>()
                .Build();

            state.RequireForUpdate(entityQuery);
            
            state.RequireForUpdate<CellRadius>();
            state.RequireForUpdate<DefaultStopMoveWorldRadius>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            half cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;
            var stopMoveRadius = SystemAPI.GetSingleton<DefaultStopMoveWorldRadius>().Value;

            new ChangeTargetPosJob
            {
                StopMoveRadius = stopMoveRadius,
                CellRadius = cellRadius,
            }.ScheduleParallel();

        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct ChangeTargetPosJob : IJobEntity
        {
            [ReadOnly] public half StopMoveRadius;
            [ReadOnly] public half CellRadius;

            private void Execute(
                EnabledRefRO<CanMoveEntityTag> canMoveEntityTag
                , in DistanceToTarget distanceToTarget
                , ref TargetPosition targetPosition
                , EnabledRefRW<TargetPosChangedTag> targetPosChangedTag
                , DynamicBuffer<WaypointElement> waypoints)
            {
                if (!canMoveEntityTag.ValueRO) return;
                if (Hint.Likely(distanceToTarget.CurrentDistance >= this.StopMoveRadius)) return;
                if (!waypoints.TryPop(out var waypoint)) return;

                WorldMapHelper.GridPosToWorldPos(in this.CellRadius, in waypoint.Value, out float3 worldPos);
                targetPosition.Value = worldPos;
                targetPosChangedTag.ValueRW = true;

            }

        }

    }

}
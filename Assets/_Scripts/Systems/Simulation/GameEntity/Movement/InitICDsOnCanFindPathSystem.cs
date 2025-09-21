using Components.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;
using Components.Misc;
using Components.Misc.WorldMap.PathFinding;
using Core.Utilities.Extensions;
using Systems.Simulation.Misc.WorldMap.PathFinding;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Utilities.Helpers.GameEntity.Movement;

namespace Systems.Simulation.GameEntity.Movement
{
    [UpdateInGroup(typeof(PathFindingSystemGroup))]
    [BurstCompile]
    public partial struct InitICDsOnCanFindPathSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , CanFindPathTag
                    , AbsoluteDistanceXZToTarget
                    , LocalTransform
                    , MoveCommandElement
                    , CurrentWorldWaypoint
                    , DistanceToCurrentWaypoint>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<DefaultStopMoveWorldRadius>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            half defaultStopMoveWorldRadius = SystemAPI.GetSingleton<DefaultStopMoveWorldRadius>().Value;

            state.Dependency = new InitICDsJob
            {
                DefaultStopMoveWorldRadius = defaultStopMoveWorldRadius,
            }.ScheduleParallel(state.Dependency);
        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        public partial struct InitICDsJob : IJobEntity
        {
            [ReadOnly] public half DefaultStopMoveWorldRadius;

            void Execute(
                EnabledRefRW<CanMoveEntityTag> canMoveEntityTag
                , EnabledRefRO<CanFindPathTag> canFindPathTag
                , ref AbsoluteDistanceXZToTarget absDistanceXZToTarget
                , in LocalTransform transform
                , in MoveCommandElement moveCommandElement
                , ref CurrentWorldWaypoint currentWaypoint
                , ref DistanceToCurrentWaypoint distanceToCurrentWaypoint)
            {
                if (!canFindPathTag.ValueRO) return;

                canMoveEntityTag.ValueRW = true;

                AbsoluteDistanceXZToTargetHelper.SetDistance(
                    ref absDistanceXZToTarget
                    , in transform.Position
                    , in moveCommandElement.Float3);

                currentWaypoint.Value = transform.Position.Add(x: this.DefaultStopMoveWorldRadius);
                distanceToCurrentWaypoint.Value = this.DefaultStopMoveWorldRadius;
            }
        }

    }

}
using Components.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;
using Components.Misc.WorldMap.PathFinding;
using Systems.Simulation.Misc.WorldMap.PathFinding;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Utilities.Helpers.GameEntity.Movement;

namespace Systems.Simulation.Unit.Movement
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
                    , MoveCommandElement>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new InitICDsJob().ScheduleParallel(state.Dependency);
        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        public partial struct InitICDsJob : IJobEntity
        {
            void Execute(
                EnabledRefRW<CanMoveEntityTag> canMoveEntityTag
                , EnabledRefRO<CanFindPathTag> canFindPathTag
                , ref AbsoluteDistanceXZToTarget absDistanceXZToTarget
                , in LocalTransform transform
                , in MoveCommandElement moveCommandElement)
            {
                if (!canFindPathTag.ValueRO) return;

                canMoveEntityTag.ValueRW = true;

                AbsoluteDistanceXZToTargetHelper.SetDistance(
                    ref absDistanceXZToTarget
                    , in transform.Position
                    , in moveCommandElement.Float3);
            }
        }

    }

}
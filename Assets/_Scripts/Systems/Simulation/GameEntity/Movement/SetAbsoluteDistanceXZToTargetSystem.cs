using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Components.GameEntity.Movement;
using Utilities.Helpers.GameEntity.Movement;
using Components.GameEntity.Movement.MoveCommand;

namespace Systems.Simulation.GameEntity.Movement
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct SetAbsoluteDistanceXZToTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , LocalTransform
                    , AbsoluteDistanceXZToTarget
                    , MoveCommandElement>()
                .Build();

            state.RequireForUpdate(entityQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new SetAbsoluteDistanceXZ().ScheduleParallel();
        }

        [WithAll(typeof(CanMoveEntityTag))]
        [BurstCompile]
        private partial struct SetAbsoluteDistanceXZ : IJobEntity
        {
            [BurstCompile]
            private void Execute(
                in LocalTransform transform
                , ref AbsoluteDistanceXZToTarget absoluteDistanceXZToTarget
                , in MoveCommandElement moveCommandElement)
            {
                AbsoluteDistanceXZToTargetHelper.SetDistance(
                    ref absoluteDistanceXZToTarget
                    , in transform.Position
                    , in moveCommandElement.Float3);

            }

        }

    }

}
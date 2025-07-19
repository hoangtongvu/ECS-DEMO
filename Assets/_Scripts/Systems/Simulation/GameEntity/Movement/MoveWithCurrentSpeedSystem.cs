using Components.GameEntity.Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems.Simulation.GameEntity.Movement
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct MoveWithCurrentSpeedSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , LocalTransform
                    , MoveDirectionFloat2
                    , CurrentMoveSpeed>()
                .Build();

            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new MoveJob().ScheduleParallel();
        }

        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            private void Execute(
                ref PhysicsVelocity physicsVelocity
                , in CurrentMoveSpeed speed
                , in MoveDirectionFloat2 direction)
            {
                if (speed.Value == 0) return;

                float3 float3Dir = new (direction.Value.x, 0f, direction.Value.y);
                physicsVelocity.Linear = float3Dir * speed.Value;
            }

        }

    }

}
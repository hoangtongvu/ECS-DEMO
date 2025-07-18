using Components.GameEntity.Movement;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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
            new MoveJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;

            private void Execute(
                ref LocalTransform transform
                , in CurrentMoveSpeed speed
                , in MoveDirectionFloat2 direction)
            {
                if (speed.Value == 0) return;

                float3 float3Dir = new (direction.Value.x, 0f, direction.Value.y);
                transform = transform.Translate(float3Dir * speed.Value * this.DeltaTime);
            }

        }

    }

}
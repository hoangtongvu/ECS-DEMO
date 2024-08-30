using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct MoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , LocalTransform
                    , MoveDirectionFloat2
                    , MoveSpeedLinear>()
                .Build();

            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new MoveJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel();
        }

        [WithAll(typeof(CanMoveEntityTag))]
        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            public float deltaTime;

            private void Execute(
                ref LocalTransform transform
                , in MoveSpeedLinear speed
                , in MoveDirectionFloat2 direction
            )
            {
                float3 float3Dir = new (direction.Value.x, 0f, direction.Value.y);
                transform = transform.Translate(float3Dir * speed.Value * this.deltaTime);
            }
        }
    }
}


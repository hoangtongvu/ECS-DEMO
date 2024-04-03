using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct MoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MoveableState>();
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<MoveDirectionFloat2>();
            state.RequireForUpdate<MoveSpeedLinear>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new MoveJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            public float deltaTime;

            private void Execute(
                in MoveableState moveableState
                , ref LocalTransform transform
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


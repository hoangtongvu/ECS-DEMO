using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct LinearRotateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<RotateDirection>();
            state.RequireForUpdate<RotateSpeed>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new RotateJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,

            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct RotateJob : IJobEntity
        {
            public float deltaTime;

            private void Execute(
                  in RotateDirection direction
                , in RotateSpeed speed
                , ref LocalTransform transform
            )
            {
                transform = transform.Rotate(quaternion.Euler(math.radians(direction.value * speed.value * deltaTime)));
                //transform = transform.RotateX(math.radians(speed.value * deltaTime * direction.value.x));
                //transform = transform.RotateY(math.radians(speed.value * deltaTime * direction.value.y));
                //transform = transform.RotateZ(math.radians(speed.value * deltaTime * direction.value.z));
            }
        }
    }
}

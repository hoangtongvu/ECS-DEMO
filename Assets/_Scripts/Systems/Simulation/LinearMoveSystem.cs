using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct LinearMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<MoveDirection>();
            state.RequireForUpdate<MoveSpeed>();
            state.RequireForUpdate<EnableableTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // The schedule logic is simple
            // so we can just code it like this.
            // The source generator of ISystem will rewrite this code to include state.Dependency.
            new MoveJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,

            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            public float deltaTime;

            // It's best to separate parameters into many lines
            // because the number of components can be large and long.
            private void Execute(
                  in MoveDirection direction
                , in MoveSpeed speed
                , ref LocalTransform transform
                , in EnableableTag enableableTag
            )
            {
                transform = transform.Translate(direction.value * speed.value * this.deltaTime);
            }
        }
    }
}


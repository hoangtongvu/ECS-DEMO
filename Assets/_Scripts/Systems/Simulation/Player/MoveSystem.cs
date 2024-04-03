using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Components;
using Components.Player;

namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct MoveSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<MoveSpeedLinear>();
            state.RequireForUpdate<InputData>();
            state.RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new MoveJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                moveDirection = SystemAPI.GetSingleton<InputData>().MoveDirection,

            }.Schedule();
        }

        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {

            public float deltaTime;
            public MoveDirectionFloat2 moveDirection;

            private void Execute(
                in PlayerTag playerTag
                , in MoveSpeedLinear speed
                , ref LocalTransform transform
            )
            {
                float2 translateValueFloat2 = this.moveDirection.Value * speed.Value * this.deltaTime;
                transform = transform.Translate(new float3(translateValueFloat2.x, 0, translateValueFloat2.y));
            }
        }

    }
}


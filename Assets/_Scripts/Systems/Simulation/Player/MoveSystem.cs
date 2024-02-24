using Components.Player;
using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct MoveSystem : ISystem
    {
        private MoveDirection moveDirection;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<MoveDirection>();
            state.RequireForUpdate<MoveSpeed>();
            state.RequireForUpdate<Components.EnableableTag>();
            this.moveDirection = new();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            this.SetMoveDirection();
            new MoveJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                moveDirection = this.moveDirection,

            }.Schedule();
        }

        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {

            public float deltaTime;
            public MoveDirection moveDirection;

            private void Execute(
                  ref MoveDirection direction
                , in MoveSpeed speed
                , ref LocalTransform transform
                , in Components.EnableableTag enableableTag
            )
            {
                direction = this.moveDirection;
                if (this.moveDirection.Up) transform = transform.Translate(new float3(0, 0, 1) * speed.Value * this.deltaTime);
                if (this.moveDirection.Down) transform = transform.Translate(new float3(0, 0, -1) * speed.Value * this.deltaTime);
                if (this.moveDirection.Left) transform = transform.Translate(new float3(-1, 0, 0) * speed.Value * this.deltaTime);
                if (this.moveDirection.Right) transform = transform.Translate(new float3(1, 0, 0) * speed.Value * this.deltaTime);
            }
        }

        private void SetMoveDirection()
        {
            this.moveDirection.Up = Input.GetKey(KeyCode.W);
            this.moveDirection.Down = Input.GetKey(KeyCode.S);
            this.moveDirection.Right = Input.GetKey(KeyCode.D);
            this.moveDirection.Left = Input.GetKey(KeyCode.A);
        }

    }
}


using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Components.GameEntity.Movement;

namespace Systems.Simulation.Misc
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct RotateEntityToMoveDirSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    MoveDirectionFloat2
                    , LocalTransform
                    , CanMoveEntityTag>()
                .Build();

            state.RequireForUpdate(query0);
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new RotateJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel();

        }

        [WithAll(typeof(CanMoveEntityTag))]
        [BurstCompile]
        private partial struct RotateJob : IJobEntity
        {
            public float DeltaTime;
            private const float rotateSpeed = 8f;

            [BurstCompile]
            void Execute(
                in MoveDirectionFloat2 moveDir
                , ref LocalTransform transform)
            {
                // Create a 3D forward vector from the 2D move direction
                float3 rawForward = new(moveDir.Value.x, 0f, moveDir.Value.y);

                // Lerp for smoothing
                float3 forward = math.lerp(transform.Forward(), rawForward, rotateSpeed * this.DeltaTime);

                // Calculate the rotation that aligns the forward direction with the move direction
                quaternion targetRotation = quaternion.LookRotationSafe(forward, math.up());

                // Set the localTransform rotation to the target rotation
                transform.Rotation = targetRotation;

            }

        }

    }

}
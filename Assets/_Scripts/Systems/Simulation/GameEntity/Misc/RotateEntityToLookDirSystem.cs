using Unity.Entities;
using Unity.Burst;
using Unity.Transforms;
using Unity.Mathematics;
using Components.GameEntity.Misc;

namespace Systems.Simulation.GameEntity.Misc
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct RotateEntityToLookDirSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LookDirectionXZ
                    , LocalTransform>()
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

        [BurstCompile]
        private partial struct RotateJob : IJobEntity
        {
            public float DeltaTime;
            private const float rotateSpeed = 8f;

            [BurstCompile]
            void Execute(
                in LookDirectionXZ lookDirXZ
                , ref LocalTransform transform)
            {
                // Create a 3D forward vector from the 2D move direction
                float3 rawForward = new(lookDirXZ.Value.x, 0f, lookDirXZ.Value.y);

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
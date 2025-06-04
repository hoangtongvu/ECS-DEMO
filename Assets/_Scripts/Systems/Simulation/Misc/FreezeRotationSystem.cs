using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace Systems.Simulation.Misc
{
    [UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
    [BurstCompile]
    public partial struct FreezeRotationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state) => state.RequireForUpdate<RotationFreezer>();

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new FreezeRotationJob()
                .ScheduleParallel();
        }

        [BurstCompile]
        private partial struct FreezeRotationJob : IJobEntity
        {
            [BurstCompile]
            void Execute(
                ref PhysicsVelocity physicsVelocity
                , ref LocalTransform localTransform
                , in RotationFreezer rotationFreezer)
            {
                physicsVelocity.Angular.x = 0;
                physicsVelocity.Angular.z = 0;
                localTransform.Rotation.value.x = rotationFreezer.X;
                localTransform.Rotation.value.z = rotationFreezer.Z;
            }

        }

    }

}
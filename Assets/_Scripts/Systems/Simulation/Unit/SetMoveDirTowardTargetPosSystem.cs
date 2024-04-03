using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


namespace Systems.Simulation.Unit
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct SetMoveDirTowardTargetPosSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MoveableState>();
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<MoveDirectionFloat2>();
            state.RequireForUpdate<TargetPosition>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
            new SetMoveDirJob().ScheduleParallel();
        }

        [BurstCompile]
        private partial struct SetMoveDirJob : IJobEntity
        {
            private void Execute(
                in MoveableState moveableState
                , in LocalTransform transform
                , ref MoveDirectionFloat2 moveDir
                , in TargetPosition targetPosition
            )
            {
                float3 rawDir = math.normalize(targetPosition.Value - transform.Position);
                moveDir.Value.x = rawDir.x;
                moveDir.Value.y = rawDir.z;
            }
        }
    }
}


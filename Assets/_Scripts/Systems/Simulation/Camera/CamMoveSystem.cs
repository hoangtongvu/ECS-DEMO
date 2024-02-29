using Components.Camera;
using Components.CustomIdentification;
using Components.Player;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct CamMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UniqueId>();
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<AddPos>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Get Player Pos through Singleton/Query.
            // Pass Player Pos into MoveJob to calculate and set Cam Pos.

            float3 playerPos = float3.zero;

            foreach (var playerTransformRef in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PlayerTag>())
            {
                playerPos = playerTransformRef.ValueRO.Position;
            }

            new MoveJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                PlayerPos = playerPos,
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            public float deltaTime;
            public float3 PlayerPos;

            
            private void Execute(
                in UniqueId id
                , ref LocalTransform transform
                , in AddPos addPos
            )
            {
                if (id.Kind != UniqueKind.Camera) return;
                transform = LocalTransform.FromPosition(this.PlayerPos + addPos.Value);
            }
        }
    }
}


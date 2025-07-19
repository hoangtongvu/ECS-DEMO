using Unity.Burst;
using Unity.Entities;
using Components.Player;
using Unity.Mathematics;
using Components.GameEntity.Movement;
using Components.Misc;

namespace Systems.Simulation.Player.Movement
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct MoveInputHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , MoveDirectionFloat2>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<InputData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float2 inputMoveDirection = SystemAPI.GetSingleton<InputData>().MoveDirection.Value;

            foreach (var (canMoveEntityTag, moveDirRef) in
                SystemAPI.Query<
                    EnabledRefRW<CanMoveEntityTag>
                    , RefRW<MoveDirectionFloat2>>()
                    .WithAll<PlayerTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (inputMoveDirection.x == 0 && inputMoveDirection.y == 0)
                {
                    canMoveEntityTag.ValueRW = false;
                    return;
                }

                canMoveEntityTag.ValueRW = true;
                moveDirRef.ValueRW.Value = math.normalize(inputMoveDirection);
                
            }

        }

    }

}
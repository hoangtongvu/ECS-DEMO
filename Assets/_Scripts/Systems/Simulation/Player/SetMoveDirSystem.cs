using Unity.Burst;
using Unity.Entities;
using Components;
using Components.Player;
using Unity.Mathematics;

namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct SetMoveDirSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , SelfEntityRef>()
                .Build();

            state.RequireForUpdate(entityQuery);

            state.RequireForUpdate<InputData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float2 inputMoveDirection = SystemAPI.GetSingleton<InputData>().MoveDirection.Value;

            foreach (var (moveDirRef, selfEntityRefRef) in
                SystemAPI.Query<
                    RefRW<MoveDirectionFloat2>
                    , RefRO<SelfEntityRef>>()
                    .WithAll<PlayerTag>())
            {
                if (inputMoveDirection.x == 0 && inputMoveDirection.y == 0)
                {
                    SystemAPI.SetComponentEnabled<MoveableState>(selfEntityRefRef.ValueRO.Value, false);
                    return;
                }

                SystemAPI.SetComponentEnabled<MoveableState>(selfEntityRefRef.ValueRO.Value, true);
                moveDirRef.ValueRW.Value = inputMoveDirection;
                
            }

        }

    }
}


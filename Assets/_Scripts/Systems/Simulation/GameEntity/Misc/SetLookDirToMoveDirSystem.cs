using Unity.Entities;
using Unity.Burst;
using Components.GameEntity.Misc;
using Components.GameEntity.Damage;
using Components.GameEntity.Movement;

namespace Systems.Simulation.GameEntity.Misc
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(RotateEntityToLookDirSystem))]
    [BurstCompile]
    public partial struct SetLookDirToMoveDirSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LookDirectionXZ
                    , MoveDirectionFloat2>()
                .WithAll<
                    CanMoveEntityTag>()
                .WithAll<
                    IsAlive>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (lookDirXZRef, moveDirRef) in SystemAPI
                .Query<
                    RefRW<LookDirectionXZ>
                    , RefRO<MoveDirectionFloat2>>()
                .WithAll<
                    CanMoveEntityTag>()
                .WithAll<
                    IsAlive>())
            {
                lookDirXZRef.ValueRW.Value = moveDirRef.ValueRO.Value;
            }

        }

    }

}
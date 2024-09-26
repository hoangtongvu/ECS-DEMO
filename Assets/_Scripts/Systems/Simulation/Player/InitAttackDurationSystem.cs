using Components.Player;
using Unity.Burst;
using Unity.Entities;
using Components.MyEntity.EntitySpawning;

namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct InitAttackDurationSystem : ISystem
    {
        private const float ATTACK_DURATION = 2/3f;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    AttackData
                    , PlayerTag
                    , NewlySpawnedTag>()
                .Build();
            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var attackDataRef in
                SystemAPI.Query<
                    RefRW<AttackData>>()
                    .WithAll<PlayerTag>()
                    .WithAll<NewlySpawnedTag>())
            {
                attackDataRef.ValueRW.attackDurationSecond = ATTACK_DURATION;
            }
        }


    }
}

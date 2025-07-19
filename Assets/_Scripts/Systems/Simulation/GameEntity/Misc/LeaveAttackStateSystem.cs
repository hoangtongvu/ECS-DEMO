using Unity.Entities;
using Unity.Burst;
using Components.GameEntity.Misc;
using Components.GameEntity.Damage;

namespace Systems.Simulation.GameEntity.Misc
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct LeaveAttackStateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    InAttackStateTimeStamp
                    , InAttackStateTag>()
                .WithAll<
                    IsAliveTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (inAttackStateTimeStamp, inAttackStateTag) in SystemAPI
                .Query<
                    RefRO<InAttackStateTimeStamp>
                    , EnabledRefRW<InAttackStateTag>>()
                .WithAll<
                    IsAliveTag>())
            {
                var elapsedSeconds = SystemAPI.Time.ElapsedTime - inAttackStateTimeStamp.ValueRO.Value;
                const float timeLimitSeconds = 3f;

                if (elapsedSeconds < timeLimitSeconds) continue;
                inAttackStateTag.ValueRW = false;

            }

        }

    }

}
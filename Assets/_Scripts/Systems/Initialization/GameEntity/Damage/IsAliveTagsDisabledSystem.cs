using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct IsAliveTagsDisabledSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    CurrentHp
                    , IsAliveTag>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new DisableTagsJob()
                .ScheduleParallel();
        }

        [BurstCompile]
        private partial struct DisableTagsJob : IJobEntity
        {
            void Execute(
                in CurrentHp currentHp
                , EnabledRefRW<IsAliveTag> isAliveTag)
            {
                if (currentHp.Value != 0) return;
                isAliveTag.ValueRW = false;
            }

        }

    }

}
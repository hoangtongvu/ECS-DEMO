using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct HpChangeHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    CurrentHp
                    , MaxHp
                    , HpChangeRecordElement
                    , IsAliveTag>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new HpChangeJob()
                .ScheduleParallel();
        }

        [BurstCompile]
        private partial struct HpChangeJob : IJobEntity
        {
            void Execute(
                ref CurrentHp currentHp
                , in MaxHp maxHp
                , ref DynamicBuffer<HpChangeRecordElement> hpChangeRecords
                , EnabledRefRW<IsAliveTag> aliveTag)
            {
                int length = hpChangeRecords.Length;
                if (length == 0) return;

                for (int i = 0; i < length; i++)
                {
                    var hpChangeRecord = hpChangeRecords[i];

                    int rawCurrentHp = currentHp.Value;
                    rawCurrentHp += hpChangeRecord.Value;

                    if (rawCurrentHp > maxHp.Value)
                        currentHp.Value = maxHp.Value;
                    else if (rawCurrentHp <= 0)
                    {
                        currentHp.Value = 0;
                        aliveTag.ValueRW = false;
                    }
                    else
                        currentHp.Value = rawCurrentHp;

                }

                hpChangeRecords.Clear();

            }

        }

    }

}
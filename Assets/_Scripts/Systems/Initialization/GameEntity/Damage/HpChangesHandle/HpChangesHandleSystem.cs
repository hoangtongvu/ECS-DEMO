using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage.HpChangesHandle
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct HpChangesHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    CurrentHp
                    , HpDataHolder
                    , HpChangeRecordElement
                    , FrameHpChange
                    , IsAlive>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new HpChangesHandleJob()
                .ScheduleParallel();
        }

        [WithAll(typeof(IsAlive))]
        [BurstCompile]
        private partial struct HpChangesHandleJob : IJobEntity
        {
            void Execute(
                ref CurrentHp currentHp
                , in HpDataHolder hpDataHolder
                , ref DynamicBuffer<HpChangeRecordElement> hpChangeRecords
                , ref FrameHpChange frameHpChange)
            {
                frameHpChange.Value = 0;

                int length = hpChangeRecords.Length;
                if (length == 0) return;

                for (int i = 0; i < length; i++)
                {
                    var hpChangeRecord = hpChangeRecords[i];
                    frameHpChange.Value += hpChangeRecord.Value;

                    int rawCurrentHp = currentHp.Value;
                    rawCurrentHp += hpChangeRecord.Value;

                    if (rawCurrentHp > hpDataHolder.Value.MaxHp)
                        currentHp.Value = hpDataHolder.Value.MaxHp;
                    else if (rawCurrentHp <= 0)
                    {
                        currentHp.Value = 0;
                        break;
                    }
                    else
                        currentHp.Value = rawCurrentHp;

                }

            }

        }

    }

}
using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage.HpChangesHandle
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(AliveToPendingDeadHandleSystem))]
    [BurstCompile]
    public partial struct HpChangeRecordsClearSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    CurrentHp
                    , HpDataHolder
                    , HpChangeRecordElement
                    , IsAliveTag>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new RecordsClearJob()
                .ScheduleParallel();
        }

        [WithAll(typeof(IsAliveTag))]
        [BurstCompile]
        private partial struct RecordsClearJob : IJobEntity
        {
            void Execute(
                ref DynamicBuffer<HpChangeRecordElement> hpChangeRecords)
            {
                hpChangeRecords.Clear();
            }

        }

    }

}
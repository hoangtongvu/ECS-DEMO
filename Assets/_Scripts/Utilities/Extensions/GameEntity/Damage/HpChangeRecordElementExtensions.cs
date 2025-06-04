using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Utilities.Extensions.GameEntity.Damage
{
    [BurstCompile]
    public static class HpChangeRecordElementExtensions
    {
        [BurstCompile]
        public static void AddRecord(
            ref this DynamicBuffer<HpChangeRecordElement> hpChangeRecords
            , int value)
        {
            hpChangeRecords.Add(new HpChangeRecordElement
            {
                Value = value
            });
        }

        [BurstCompile]
        public static void AddDeductRecord(
            ref this DynamicBuffer<HpChangeRecordElement> hpChangeRecords
            , int value)
        {
            hpChangeRecords.Add(new HpChangeRecordElement
            {
                Value = -value
            });
        }

    }

}

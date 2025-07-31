using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(HpChangesHandleSystem))]
    [BurstCompile]
    public partial struct NewlyTakeHitTagHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    HpChangeRecordElement>()
                .WithAll<
                    NewlyTakeHitTag>()
                .WithAll<
                    IsAliveTag>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var newlyTakeHitTag in SystemAPI
                .Query<
                    EnabledRefRW<NewlyTakeHitTag>>())
            {
                newlyTakeHitTag.ValueRW = false;
            }

            foreach (var (hpChangeRecords, entity) in SystemAPI
                .Query<
                    DynamicBuffer<HpChangeRecordElement>>()
                .WithAll<
                    IsAliveTag>()
                .WithEntityAccess())
            {
                if (hpChangeRecords.Length == 0) continue;
                SystemAPI.SetComponentEnabled<NewlyTakeHitTag>(entity, true);
            }

        }

    }

}
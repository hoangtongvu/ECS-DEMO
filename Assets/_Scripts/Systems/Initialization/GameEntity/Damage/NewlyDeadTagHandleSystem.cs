using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(HpChangesHandleSystem))]
    [BurstCompile]
    public partial struct NewlyDeadTagHandleSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    CurrentHp
                    , NewlyDeadTag>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.SetComponentEnabled<NewlyDeadTag>(this.query, false);

            foreach (var (currentHpRef, isAliveTag, newlyDeadTag) in SystemAPI
                .Query<
                    RefRO<CurrentHp>
                    , EnabledRefRW<IsAliveTag>
                    , EnabledRefRW<NewlyDeadTag>>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!isAliveTag.ValueRO) continue;
                if (currentHpRef.ValueRO.Value != 0) continue;
                newlyDeadTag.ValueRW = true;
            }

        }

    }

}
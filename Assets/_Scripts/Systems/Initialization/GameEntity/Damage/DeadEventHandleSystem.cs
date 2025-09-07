using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(HpChangesHandleSystem))]
    [BurstCompile]
    public partial struct DeadEventHandleSystem : ISystem
    {
        private EntityQuery query0;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CurrentHp
                    , DeadEvent>()
                .Build();

            state.RequireForUpdate(this.query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.SetComponentEnabled<DeadEvent>(this.query0, false);

            // NOTE: This check is essential as not all dead entities are destroyed automatically or within the next frame
            foreach (var (currentHpRef, entity) in SystemAPI
                .Query<
                    RefRO<CurrentHp>>()
                .WithAll<
                    IsAliveTag>()
                .WithEntityAccess())
            {
                bool isDead = currentHpRef.ValueRO.Value == 0;
                if (!isDead) continue;

                SystemAPI.SetComponentEnabled<DeadEvent>(entity, true);
            }

        }

    }

}
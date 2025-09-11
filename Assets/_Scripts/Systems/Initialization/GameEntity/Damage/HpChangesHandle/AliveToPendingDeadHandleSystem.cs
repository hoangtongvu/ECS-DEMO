using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage.HpChangesHandle
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct AliveToPendingDeadHandleSystem : ISystem
    {
        private EntityQuery query0;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CurrentHp
                    , IsAlive>()
                .Build();

            state.RequireForUpdate(this.query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query0.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;

            var em = state.EntityManager;
            var currentHpArray = this.query0.ToComponentDataArray<CurrentHp>(Allocator.Temp);

            for (int i = 0; i < length; i++)
            {
                var entity = entities[i];
                var currentHp = currentHpArray[i].Value;

                if (currentHp != 0) continue;

                em.RemoveComponent<IsAlive>(entity);
                em.AddComponent<PendingDead>(entity);
            }
        }

    }

}
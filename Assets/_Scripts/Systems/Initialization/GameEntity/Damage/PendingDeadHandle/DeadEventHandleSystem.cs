using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage.PendingDeadHandle
{
    [UpdateInGroup(typeof(PendingDeadHandleSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(PendingDeadTagsClearSystem))]
    [BurstCompile]
    public partial struct DeadEventHandleSystem : ISystem
    {
        private EntityQuery query0;
        private EntityQuery query1;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CurrentHp
                    , DeadEvent>()
                .Build();

            this.query1 = SystemAPI.QueryBuilder()
                .WithAll<
                    PendingDead
                    , IsDead>()
                .WithDisabled<
                    DeadEvent>()
                .Build();

            state.RequireForUpdate(this.query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            em.SetComponentEnabled<DeadEvent>(this.query0, false);
            em.SetComponentEnabled<DeadEvent>(this.query1, true);
        }

    }

}
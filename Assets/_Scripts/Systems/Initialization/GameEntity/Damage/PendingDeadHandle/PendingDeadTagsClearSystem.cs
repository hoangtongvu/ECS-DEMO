using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage.PendingDeadHandle
{
    [UpdateInGroup(typeof(PendingDeadHandleSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct PendingDeadTagsClearSystem : ISystem
    {
        private EntityQuery query0;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PendingDead
                    , IsDead>()
                .Build();

            state.RequireForUpdate(this.query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.RemoveComponent<PendingDead>(this.query0);
        }

    }

}
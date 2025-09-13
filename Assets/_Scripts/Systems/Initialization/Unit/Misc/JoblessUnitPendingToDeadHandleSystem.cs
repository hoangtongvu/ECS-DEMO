using Components.GameEntity.Damage;
using Components.Unit;
using Systems.Initialization.GameEntity.Damage.PendingDeadHandle;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.Unit.Misc
{
    [UpdateInGroup(typeof(PendingDeadHandleSystemGroup))]
    [BurstCompile]
    public partial struct JoblessUnitPendingToDeadHandleSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<JoblessUnitTag>()
                .WithAll<PendingDead>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            em.AddComponent<IsDead>(this.query);
        }

    }

}
using Components.GameEntity.Damage;
using Components.Player;
using Systems.Initialization.GameEntity.Damage.PendingDeadHandle;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.Player.Misc
{
    [UpdateInGroup(typeof(PendingDeadHandleSystemGroup))]
    [BurstCompile]
    public partial struct PlayerPendingToDeadHandleSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<PlayerTag>()
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
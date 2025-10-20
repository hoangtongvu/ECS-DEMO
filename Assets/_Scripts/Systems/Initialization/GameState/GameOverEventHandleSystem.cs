using Components.GameEntity.Damage;
using Components.GameState;
using Components.Player;
using Systems.Initialization.GameEntity.Damage.DeadResolve;
using Unity.Burst;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.GameState
{
    [UpdateInGroup(typeof(DeadResolveSystemGroup))]
    [BurstCompile]
    public partial struct GameOverEventHandleSystem : ISystem
    {
        private EntityQuery deadPlayerQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var su = SingletonUtilities.GetInstance(state.EntityManager);
            su.AddAndDisableComponent<GameOverEvent>();

            this.deadPlayerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag>()
                .WithAll<
                    DeadEvent>()
                .Build();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var su = SingletonUtilities.GetInstance(state.EntityManager);
            su.SetComponentEnabled<GameOverEvent>(false);

            bool isPlayerDead = this.deadPlayerQuery.CalculateEntityCount() != 0;
            if (!isPlayerDead) return;

            su.SetComponentEnabled<GameOverEvent>(true);
        }

    }

}
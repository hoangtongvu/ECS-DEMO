using Components.GameEntity.Damage;
using Components.GameState;
using Components.Player;
using Systems.Initialization.GameEntity.Damage.DeadResolve;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.GameState
{
    [UpdateInGroup(typeof(DeadResolveSystemGroup))]
    [BurstCompile]
    public partial struct SetIsGameOverSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag>()
                .WithAll<
                    DeadEvent>()
                .Build();

            state.RequireForUpdate(this.query);
            state.RequireForUpdate<IsGameStarted>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            if (entities.Length == 0) return;

            var su = SingletonUtilities.GetInstance(state.EntityManager);

            su.AddComponent<IsGameOver>();
            su.TryRemoveComponent<IsGameStarted>();
        }

    }

}
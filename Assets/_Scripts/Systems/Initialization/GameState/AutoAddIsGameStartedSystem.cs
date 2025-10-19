using Components.GameState;
using Systems.Initialization.GameEntity.Damage.DeadResolve;
using Unity.Burst;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.GameState
{
    [UpdateInGroup(typeof(DeadResolveSystemGroup))]
    [BurstCompile]
    public partial struct AutoAddIsGameStartedSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var su = SingletonUtilities.GetInstance(state.EntityManager);
            su.AddComponent<IsGameStarted>();
        }

    }

}
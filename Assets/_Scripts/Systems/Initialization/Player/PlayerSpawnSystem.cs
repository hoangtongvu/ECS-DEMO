using Components.GameEntity;
using Components.Player;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.Player
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct PlayerSpawnSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerProfilesSOHolder
                    , AfterBakedPrefabsElement>()
                .Build();

            state.RequireForUpdate(this.query);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var profilesSOHolder = this.query.GetSingleton<PlayerProfilesSOHolder>();
            var afterBakedPrefabsBuffer = this.query.GetSingletonBuffer<AfterBakedPrefabsElement>();

            state.EntityManager.Instantiate(afterBakedPrefabsBuffer[0].PrimaryEntity);


        }

    }

}
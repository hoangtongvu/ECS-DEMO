using Components.GameEntity;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.Misc.Presenter.PresenterPrefabGO
{
    [UpdateInGroup(typeof(NewlySpawnedTagProcessSystemGroup))]
    [BurstCompile]
    public partial struct AddGameEntitySizeHolderSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    PrimaryPrefabEntityHolder>()
                .WithAll<
                    NewlySpawnedTag>()
                .Build();

            state.RequireForUpdate(this.query);
            state.RequireForUpdate<GameEntitySizeMap>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;

            var em = state.EntityManager;
            var gameEntitySizeMap = SystemAPI.GetSingleton<GameEntitySizeMap>().Value;
            var primaryPrefabEntities = this.query.ToComponentDataArray<PrimaryPrefabEntityHolder>(Allocator.Temp);

            for (int i = 0; i < length; i++)
            {
                var entity = entities[i];
                var primaryPrefabEntity = primaryPrefabEntities[i];

                if (!gameEntitySizeMap.TryGetValue(primaryPrefabEntity, out var gameEntitySize)) continue;

                em.AddComponentData(entity, new GameEntitySizeHolder
                {
                    Value = gameEntitySize,
                });

            }

        }

    }

}
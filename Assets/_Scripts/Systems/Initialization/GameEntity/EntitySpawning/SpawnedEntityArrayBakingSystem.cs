using Components.GameEntity.EntitySpawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Scenes;

namespace Systems.Baking.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SceneSystemGroup))]
    [BurstCompile]
    public partial struct SpawnedEntityArrayBakingSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    EntitySpawningProfileElement
                    , SpawnedEntityCountLimit>()
                .WithAll<
                    Prefab>()
                .Build();

            state.RequireForUpdate(this.entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var entities = this.entityQuery.ToEntityArray(Allocator.Temp);
            var spawnedEntityCountLimits = this.entityQuery.ToComponentDataArray<SpawnedEntityCountLimit>(Allocator.Temp);
            int length = entities.Length;

            var em = state.EntityManager;

            for (int i = 0; i < length; i++)
            {
                var entity = entities[i];
                byte spawnedCountLimit = spawnedEntityCountLimits[i].Value;

                em.AddComponentData(entity, new SpawnedEntityArray
                {
                    Value = new(spawnedCountLimit, Allocator.Persistent),
                });

            }

        }

    }

}

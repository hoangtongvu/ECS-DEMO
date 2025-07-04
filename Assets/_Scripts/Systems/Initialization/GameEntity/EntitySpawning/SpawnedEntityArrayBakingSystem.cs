using Components.GameEntity.EntitySpawning;
using Systems.Initialization.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Baking.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(NewlySpawnedTagProcessSystemGroup))]
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
                    NewlySpawnedTag>()
                .Build();

            state.RequireForUpdate(this.entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.entityQuery.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;

            var spawnedEntityCountLimits = this.entityQuery.ToComponentDataArray<SpawnedEntityCountLimit>(Allocator.Temp);
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

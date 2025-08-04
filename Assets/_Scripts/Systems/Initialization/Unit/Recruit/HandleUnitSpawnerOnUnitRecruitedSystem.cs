using Components.GameEntity.EntitySpawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities.Extensions.GameEntity.EntitySpawning;
using Components.Unit.Recruit;

namespace Systems.Initialization.Unit.Recruit
{
    [UpdateInGroup(typeof(RecruitableTagsHandleSystemGroup))]
    [BurstCompile]
    public partial struct HandleUnitSpawnerOnUnitRecruitedSystem : ISystem
    {
        private EntityQuery recruitableQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.recruitableQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    NewlyRecruitedTag>()
                .WithAll<
                    SpawnerEntityHolder>()
                .Build();

            state.RequireForUpdate(this.recruitableQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.recruitableQuery.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;

            var spawnerEntityHolders = this.recruitableQuery.ToComponentDataArray<SpawnerEntityHolder>(Allocator.Temp);

            for (int i = 0; i < length; i++)
            {
                var entity = entities[i];
                var spawnerEntity = spawnerEntityHolders[i].Value;

                this.HandleUnitSpawner(
                    ref state
                    , in spawnerEntity
                    , in entity);

            }

        }

        [BurstCompile]
        private void HandleUnitSpawner(
            ref SystemState state
            , in Entity spawnerEntity
            , in Entity entity)
        {
            if (SystemAPI.HasComponent<SpawnedEntityCounter>(spawnerEntity))
            {
                var spawnedEntityCounterRef = SystemAPI.GetComponentRW<SpawnedEntityCounter>(spawnerEntity);
                spawnedEntityCounterRef.ValueRW.Value--;
            }

            if (SystemAPI.HasComponent<SpawnedEntityArray>(spawnerEntity))
            {
                var spawnedEntities = SystemAPI.GetComponent<SpawnedEntityArray>(spawnerEntity);
                spawnedEntities.Remove(in entity);
            }
            
        }

    }

}
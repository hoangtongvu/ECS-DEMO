using Components.GameEntity.EntitySpawning;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities.Extensions.GameEntity.EntitySpawning;
using Components.GameEntity.Damage;
using Components.Unit.Misc;
using Systems.Initialization.GameEntity.Damage.DeadResolve;

namespace Systems.Initialization.Unit.Misc
{
    [UpdateInGroup(typeof(DeadResolveSystemGroup))]
    [BurstCompile]
    public partial struct UnlinkDeadUnitFromSpawnerSystem : ISystem
    {
        private EntityQuery targetEntityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.targetEntityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    DeadEvent>()
                .WithAll<
                    SpawnerEntityHolder>()
                .WithAll<
                    UnitTag>()
                .Build();

            state.RequireForUpdate(this.targetEntityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.targetEntityQuery.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;

            var spawnerEntityHolders = this.targetEntityQuery.ToComponentDataArray<SpawnerEntityHolder>(Allocator.Temp);

            for (int i = 0; i < length; i++)
            {
                var spawnerEntity = spawnerEntityHolders[i].Value;
                if (spawnerEntity == Entity.Null) continue;

                var entity = entities[i];

                this.UnlinkUnitFromSpawner(
                    ref state
                    , in spawnerEntity
                    , in entity);
            }

        }

        [BurstCompile]
        private void UnlinkUnitFromSpawner(
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
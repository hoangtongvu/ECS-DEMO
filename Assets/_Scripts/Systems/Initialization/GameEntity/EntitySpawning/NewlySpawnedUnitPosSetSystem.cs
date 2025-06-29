using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Misc;
using Core.Utilities.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Initialization.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SpawnPrefabSystem))]
    [BurstCompile]
    public partial struct NewlySpawnedUnitPosSetSystem : ISystem
    {
        private Random rand;
        private float2 tempVector;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.rand = new Random(1);
            this.tempVector = new(1, 1);

            var entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    NewlySpawnedTag
                    , NeedInitPosAroundSpawnerTag
                    , LocalTransform
                    , SpawnerEntityHolder>()
                .Build();

            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = new NativeList<Entity>(10, Allocator.Temp);

            foreach (var (spawnerEntityHolderRef, transformRef, entity) in SystemAPI
                .Query<
                    RefRO<SpawnerEntityHolder>
                    , RefRW<LocalTransform>>()
                .WithAll<
                    NewlySpawnedTag
                    , NeedInitPosAroundSpawnerTag>()
                .WithEntityAccess())
            {
                entities.Add(entity);
                var spawnerEntity = spawnerEntityHolderRef.ValueRO.Value;
                if (spawnerEntity == Entity.Null) continue;

                var spawnerTransform = SystemAPI.GetComponent<LocalTransform>(spawnerEntity);

                transformRef.ValueRW.Position = this.GetRandomPositionInRadius(3, spawnerTransform.Position); // TODO: Find another way to get this value
            }

            state.EntityManager.RemoveComponent<NeedInitPosAroundSpawnerTag>(entities.AsArray());

        }

        private float3 GetRandomPositionInRadius(in float radius, in float3 centerPos)
        {
            // This is heavy work?? try to use job.
            float2 distanceVector = math.normalize(this.rand.NextFloat2(-this.tempVector, this.tempVector)) * radius;
            return centerPos.Add(x: distanceVector.x, z: distanceVector.y);
        }

    }

}
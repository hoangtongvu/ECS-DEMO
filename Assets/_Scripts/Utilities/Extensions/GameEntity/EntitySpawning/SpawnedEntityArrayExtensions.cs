using Components.GameEntity.EntitySpawning;
using System;
using Unity.Burst;
using Unity.Entities;

namespace Utilities.Extensions.GameEntity.EntitySpawning
{
    [BurstCompile]
    public static class SpawnedEntityArrayExtensions
    {
        [BurstCompile]
        public static void Add(this ref SpawnedEntityArray spawnedEntityArray, in Entity entity)
        {
            int length = spawnedEntityArray.Value.Length;

            for (int i = 0; i < length; i++)
            {
                if (spawnedEntityArray.Value[i] != Entity.Null) continue;
                spawnedEntityArray.Value[i] = entity;
                return;
            }

            throw new System.Exception($"Can not add {entity.ToFixedString()} because {nameof(SpawnedEntityArray)} is full");
        }

        [BurstCompile]
        public static void Remove(this ref SpawnedEntityArray spawnedEntityArray, in Entity entity)
        {
            int length = spawnedEntityArray.Value.Length;

            for (int i = 0; i < length; i++)
            {
                if (spawnedEntityArray.Value[i] != entity) continue;
                spawnedEntityArray.Value[i] = Entity.Null;
                return;
            }

            throw new NullReferenceException($"Can not remove non-existing element {entity.ToFixedString()} in {nameof(SpawnedEntityArray)}");
        }

        [BurstCompile]
        public static void Clear(this ref SpawnedEntityArray spawnedEntityArray)
        {
            int length = spawnedEntityArray.Value.Length;

            for (int i = 0; i < length; i++)
            {
                spawnedEntityArray.Value[i] = Entity.Null;
            }

        }

    }

}

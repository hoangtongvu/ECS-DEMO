using Authoring.Utilities.Extensions;
using Authoring.Utilities.Helpers.GameBuilding;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.EntitySpawning.SpawningProcess;
using Core.GameEntity;
using System;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameEntity.EntitySpawning
{
    public class EntitySpawningAuthoring : MonoBehaviour
    {
        public float SpawnRadius = 3f;
        public EntitySpawningPrefabsSO SpawningPrefabs;

        private class Baker : Baker<EntitySpawningAuthoring>
        {
            public override void Bake(EntitySpawningAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                GameBuildingBakingHelper.AddComponents(this, entity);

                AddComponent<SpawnedEntityCounter>(entity);
                AddComponent(entity, new SpawnedEntityCountLimit
                {
                    Value = 3
                });

                this.AddAndDisableComponent<IsInSpawningProcessTag>(entity);
                this.AddAndDisableComponent<JustBeginSpawningProcessTag>(entity);
                this.AddAndDisableComponent<JustEndSpawningProcessTag>(entity);

                var buffer = AddBuffer<EntitySpawningProfileElement>(entity);

                if (authoring.SpawningPrefabs == null)
                    throw new NullReferenceException($"{nameof(authoring.SpawningPrefabs)} is null");

                foreach (var prefab in authoring.SpawningPrefabs.Prefabs)
                {
                    buffer.Add(new()
                    {
                        PrefabToSpawn = GetEntity(prefab.Value, TransformUsageFlags.Dynamic),
                        CanSpawnState = false,
                        SpawnCount = new()
                        {
                            Value = 0,
                            ValueChanged = false,
                        },
                        DurationCounterSeconds = 0f,

                    });

                }

            }

        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, this.SpawnRadius);
        }

    }

}

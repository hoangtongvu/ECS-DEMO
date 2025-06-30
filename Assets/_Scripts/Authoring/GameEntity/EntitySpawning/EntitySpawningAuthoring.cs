using Authoring.Utilities.Helpers.GameEntity.InteractableActions;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Misc;
using Components.Misc;
using Components.Misc.Presenter;
using Components.Misc.Presenter.PresenterPrefabGO;
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

                AddComponent<NewlySpawnedTag>(entity);
                AddComponent<NeedSpawnPresenterTag>(entity);
                AddComponent<WithinPlayerAutoInteractRadiusTag>(entity);
                SetComponentEnabled<WithinPlayerAutoInteractRadiusTag>(entity, false);

                AddComponent<SpawnedEntityCounter>(entity);
                AddComponent(entity, new SpawnedEntityCountLimit
                {
                    Value = 3
                });

                AddComponent(entity, FactionIndex.Neutral);
                AddComponent<SpawnerEntityHolder>(entity);

                AddSharedComponent<PresenterOriginalMaterialHolder>(entity, default);
                InteractableActionsBakingHelper.AddComponents(this, entity);

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

﻿using Components.MyEntity.EntitySpawning;
using Core.GameResource;
using Core.MyEntity;
using System;
using Unity.Entities;
using UnityEngine;

namespace Authoring.MyEntity.EntitySpawning
{
    public class EntitySpawningAuthoring : MonoBehaviour
    {
        public float SpawnRadius = 3f;
        public EntitySpawningProfilesSO SpawningProfiles;

        private class Baker : Baker<EntitySpawningAuthoring>
        {
            public override void Bake(EntitySpawningAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                var buffer = AddBuffer<EntitySpawningProfileElement>(entity);

                if (authoring.SpawningProfiles == null)
                    throw new NullReferenceException($"{nameof(authoring.SpawningProfiles)} is null");

                foreach (var profile in authoring.SpawningProfiles.GetProfiles())
                {
                    buffer.Add(new()
                    {
                        PrefabToSpawn = GetEntity(profile.Prefab, TransformUsageFlags.Dynamic),
                        UnitSprite = profile.ProfilePicture,
                        CanSpawnState = false,

                        CanIncSpawnCount = true,

                        SpawnCount = new()
                        {
                            Value = 0,
                            ValueChanged = false,
                        },

                        SpawnDuration = new()
                        {
                            SpawnDurationSeconds = profile.SpawnDurationSeconds,
                            DurationCounterSeconds = 0,
                        },

                    });
                }
                

                var localCostBuffer = AddBuffer<LocalCostMapElement>(entity);

                int enumLength = Enum.GetNames(typeof(ResourceType)).Length;
                foreach (var profile in authoring.SpawningProfiles.GetProfiles())
                {
                    for (int i = 0; i < enumLength; i++)
                    {
                        profile.BaseCosts.TryGetValue((ResourceType)i, out var tempCost);

                        localCostBuffer.Add(new()
                        {
                            Cost = tempCost,
                        });

                    }
                }

            }
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, this.SpawnRadius);
        }

    }

}

using Components.MyEntity.EntitySpawning;
using Core.GameResource;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Authoring.MyEntity.EntitySpawning
{
    public class EntitySpawningAuthoring : MonoBehaviour
    {
        public float SpawnRadius = 3f;
        public List<Core.MyEntity.SpawningProfile> SpawningProfiles;


        private class Baker : Baker<EntitySpawningAuthoring>
        {
            public override void Bake(EntitySpawningAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                var buffer = AddBuffer<EntitySpawningProfileElement>(entity);

                foreach (var profile in authoring.SpawningProfiles)
                {
                    buffer.Add(new()
                    {
                        PrefabToSpawn = GetEntity(profile.EntityProfileSO.Prefab, TransformUsageFlags.Dynamic),
                        UnitSprite = profile.EntityProfileSO.ProfilePicture,
                        CanSpawnState = profile.CanSpawn,
                        SpawnCount = profile.SpawnCount,
                        SpawnDuration = new()
                        {
                            DurationPerSpawn = profile.EntityProfileSO.DurationPerSpawn,
                            DurationCounterSecond = 0,
                        },

                    });
                }
                

                var localCostBuffer = AddBuffer<LocalCostMapElement>(entity);

                int enumLength = Enum.GetNames(typeof(ResourceType)).Length;
                foreach (var profile in authoring.SpawningProfiles)
                {
                    for (int i = 0; i < enumLength; i++)
                    {
                        profile.EntityProfileSO.BaseCosts.TryGetValue((ResourceType)i, out var tempCost);

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

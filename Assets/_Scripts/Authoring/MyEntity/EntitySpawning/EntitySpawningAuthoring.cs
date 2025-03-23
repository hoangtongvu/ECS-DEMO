using Components.MyEntity.EntitySpawning;
using Core.GameResource;
using Core.MyEntity;
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Authoring.MyEntity.EntitySpawning
{
    public class EntitySpawningAuthoring : MonoBehaviour
    {
        public float SpawnRadius = 3f;
        public List<EntityProfileSO> EntityProfileSOs;

        private class Baker : Baker<EntitySpawningAuthoring>
        {
            public override void Bake(EntitySpawningAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                var buffer = AddBuffer<EntitySpawningProfileElement>(entity);

                foreach (var profile in authoring.EntityProfileSOs)
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
                            DurationPerSpawn = profile.DurationPerSpawn,
                            DurationCounterSecond = 0,
                        },

                    });
                }
                

                var localCostBuffer = AddBuffer<LocalCostMapElement>(entity);

                int enumLength = Enum.GetNames(typeof(ResourceType)).Length;
                foreach (var profile in authoring.EntityProfileSOs)
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

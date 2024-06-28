using Components.Unit.UnitSpawning;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit.UnitSpawning
{
    public class UnitSpawningAuthoring : MonoBehaviour
    {
        public float SpawnRadius = 3f;
        public List<Core.Unit.SpawningProfile> SpawningProfiles;


        private class Baker : Baker<UnitSpawningAuthoring>
        {
            public override void Bake(UnitSpawningAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new SpawnRadius
                {
                    Value = authoring.SpawnRadius,
                });

                var buffer = AddBuffer<UnitSpawningProfileElement>(entity);

                foreach (var profile in authoring.SpawningProfiles)
                {
                    buffer.Add(new()
                    {
                        PrefabToSpawn = GetEntity(profile.UnitProfileSO.Prefab, TransformUsageFlags.Dynamic),
                        UnitSprite = profile.UnitProfileSO.ProfilePicture,
                        CanSpawnState = profile.CanSpawn,
                        SpawnCount = profile.SpawnCount,
                        SpawnDuration = new()
                        {
                            DurationPerSpawn = profile.UnitProfileSO.DurationPerUnit,
                            DurationCounterSecond = 0,
                        },

                        UnitType = profile.UnitProfileSO.UnitType,
                        LocalIndex = profile.UnitProfileSO.LocalIndex,
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

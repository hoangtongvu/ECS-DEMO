using Components.Unit.UnitSpawning;
using Core.Unit;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit.UnitSpawning
{
    public class UnitSpawningAuthoring : MonoBehaviour
    {
        public UnitProfileSO UnitProfileSO;
        public bool CanSpawn = false;
        public int SpawnCount = 0;
        public float SpawnRadius = 3f;


        private class Baker : Baker<UnitSpawningAuthoring>
        {
            public override void Bake(UnitSpawningAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic); // Dynamic or static?? any dynamic unit can spawn?

                AddComponent(entity, new PrefabToSpawn
                {
                    Value = GetEntity(authoring.UnitProfileSO.Prefab, TransformUsageFlags.Dynamic),
                });

                AddComponent(entity, new CanSpawnState
                {
                    Value = authoring.CanSpawn,
                });

                AddComponent(entity, new SpawnCount
                {
                    Value = authoring.SpawnCount,
                });

                AddComponent(entity, new SpawnRadius
                {
                    Value = authoring.SpawnRadius,
                });

                AddComponent(entity, new SpawnDuration
                {
                    DurationPerSpawn = authoring.UnitProfileSO.DurationPerUnit,
                    DurationCounterSecond = 0,
                });


            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, this.SpawnRadius);
        }

    }
}

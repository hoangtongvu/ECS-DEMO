using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class EntitySpawnerAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int spawnCount = 1000;
        [SerializeField] private float spacing = 3f;

        private class Baker : Baker<EntitySpawnerAuthoring>
        {
            public override void Bake(EntitySpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new EntitySpawner
                {
                    prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                    spawnCount = authoring.spawnCount,
                    spacing = authoring.spacing,
                });

                AddBuffer<EntityRefElement>(entity);
            }
        }
    }
}
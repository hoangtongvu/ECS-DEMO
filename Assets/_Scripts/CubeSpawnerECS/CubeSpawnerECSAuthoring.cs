using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class CubeSpawnerECSAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private List<Transform> spawnedObj;
    [SerializeField] private int spawnCount = 1000;
    [SerializeField] private float spacing = 3f;

    public class Baker : Baker<CubeSpawnerECSAuthoring>
    {
        public override void Bake(CubeSpawnerECSAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);

            Data data = new()
            {
                entity = GetEntity(authoring.prefabToSpawn, TransformUsageFlags.Dynamic),
                //spawnedEntities = new NativeArray<Entity>(100, Allocator.Persistent),
                spawnCount = authoring.spawnCount,
                spacing = authoring.spacing,
            };

            AddComponent(entity, data);
        }
    }

    public struct Data : IComponentData
    {
        public Entity entity;
        //public NativeArray<Entity> spawnedEntities;
        public int spawnCount;
        public float spacing;
    }
}

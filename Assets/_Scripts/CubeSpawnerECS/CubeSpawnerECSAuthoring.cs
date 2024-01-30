using System.Collections;
using System.Collections.Generic;
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
                spawnCount = authoring.spawnCount,
                spacing = authoring.spacing,
            };

            AddComponent(entity, data);
        }
    }

    public struct Data : IComponentData, IEnableableComponent
    {
        public Entity entity;
        public int spawnCount;
        public float spacing;
    }
}

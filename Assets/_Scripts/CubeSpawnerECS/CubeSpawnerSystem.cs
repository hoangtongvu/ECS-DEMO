using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Scenes;
using Unity.Transforms;
using UnityEditor.SceneManagement;
using UnityEngine;

public partial class CubeSpawnerSystem : SystemBase
{

    protected override void OnCreate()
    {
        RequireForUpdate<CubeSpawnerECSAuthoring.Data>();
    }

    protected override void OnUpdate()
    {
        this.Enabled = false;// make sure this Update only run once.

        CubeSpawnerECSAuthoring.Data data = SystemAPI.GetSingleton<CubeSpawnerECSAuthoring.Data>();

        this.SpawnCubes(data);

    }

    public void SpawnCubes(CubeSpawnerECSAuthoring.Data data)
    {
        int rows = Mathf.CeilToInt(Mathf.Sqrt(data.spawnCount));
        float offset = data.spacing * 0.5f; // Adjust for center alignment

        EntityCommandBuffer buffer = new EntityCommandBuffer(WorldUpdateAllocator);

        for (int i = 0; i < data.spawnCount; i++)
        {
            int row = i / rows;
            int col = i % rows;

            float3 position = new(
                col * data.spacing - offset,
                0,
                row * data.spacing - offset
            );

            //LocalTransform transform = new()
            //{
            //    Position = position,
            //};

            LocalTransform transform = LocalTransform.FromPosition(position);

            Entity entity = buffer.Instantiate(data.entity);
            //SystemAPI.SetComponent(entity, transform);
            buffer.SetComponent(entity, transform);
            
        }

        buffer.Playback(EntityManager);

    }

}

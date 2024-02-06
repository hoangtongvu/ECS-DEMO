using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct CubeSpawnerSystem : ISystem
{

    public void OnCreate(ref SystemState state) => state.RequireForUpdate<CubeSpawnerECSAuthoring.Data>();

    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;// make sure this Update only run once.

        CubeSpawnerECSAuthoring.Data data = SystemAPI.GetSingleton<CubeSpawnerECSAuthoring.Data>();

        this.SpawnCubes(ref state, data);
    }


    private void SpawnCubes(ref SystemState state, CubeSpawnerECSAuthoring.Data data)
    {
        int rows = Mathf.CeilToInt(Mathf.Sqrt(data.spawnCount));
        float offset = data.spacing * 0.5f; // Adjust for center alignment

        NativeArray<Entity> spawnedEntities = state.EntityManager.Instantiate(data.entity, data.spawnCount, state.WorldUpdateAllocator);
        //data.spawnedEntities.CopyFrom(spawnedEntities);
        
        //Debug.Log(data.spawnedEntities);

        //Set Grid Transform.
        for (int i = 0; i < data.spawnCount; i++)
        {
            int row = i / rows;
            int col = i % rows;

            LocalTransform transform = this.GetCubeTransform(col, row, data.spacing, offset);

            SystemAPI.SetComponent(spawnedEntities[i], transform);

        }

        //Set Component Job.
        state.Dependency = default;
        SetComponentJob job = new()
        {
            entities = spawnedEntities,
            lookup = SystemAPI.GetComponentLookup<LinearMoveAuthoring.Data>(),
        };

        JobHandle jobHandle = job.ScheduleParallel(data.spawnCount, 32);
        state.Dependency = jobHandle;
        //jobHandle.Complete();
    }

    private LocalTransform GetCubeTransform(int col, int row, float spacing, float offset)
    {
        float3 position = new
        (
            (col * spacing) - offset,
            0,
            (row * spacing) - offset
        );
        return LocalTransform.FromPosition(position);
    }


}


public partial struct SetComponentJob : IJobParallelForBatch
{

    public NativeArray<Entity> entities;
    [NativeDisableParallelForRestriction] public ComponentLookup<LinearMoveAuthoring.Data> lookup;

    public void Execute(int startIndex, int count)
    {
        uint randomSeed = (uint)(startIndex + 1);// plus 1 to make seed non-zero.
        Unity.Mathematics.Random random = new(randomSeed);

        for (int i = startIndex; i < startIndex + count; i++)
        {
            Entity e = this.entities[i];

            RefRW<LinearMoveAuthoring.Data> data = this.lookup.GetRefRWOptional(e);
            if (!data.IsValid)
            {
                Debug.Log("data not found.");
                return;
            }
            data.ValueRW.direction = this.GetRandomDirection(ref random);
        }

    }

    private float3 GetRandomDirection(ref Unity.Mathematics.Random random)
    {
        return new float3(
            random.NextFloat(-1f, 1f),
            0f,
            random.NextFloat(-1f, 1f));
    }


}

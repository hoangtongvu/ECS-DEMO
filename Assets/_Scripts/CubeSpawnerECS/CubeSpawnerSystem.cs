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

        NativeArray<Entity> spawnedEntities = state.EntityManager.Instantiate(data.entity, data.spawnCount, state.WorldUpdateAllocator);

        int rows = Mathf.CeilToInt(Mathf.Sqrt(data.spawnCount));
        float offset = data.spacing * 0.5f; // Adjust for center alignment


        DynamicBuffer<CubeSpawnerECSAuthoring.EntityBufferElement> entitiesHolder =
            SystemAPI.GetBuffer<CubeSpawnerECSAuthoring.EntityBufferElement>(data.entity);// or state.SystemHandle.


        foreach (Entity entity in spawnedEntities)
        {
            CubeSpawnerECSAuthoring.EntityBufferElement element = new()
            {
                e = entity,
            };
            entitiesHolder.Add(element);
        }

        //Set Component Job.
        state.Dependency = default;
        SetComponentJob job = new()
        {
            entities = spawnedEntities,
            linearMoveLookup = SystemAPI.GetComponentLookup<LinearMoveAuthoring.Data>(),
            transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
            rows =  rows,
            offset = offset,
            spacing = data.spacing,
        };

        state.Dependency = job.ScheduleParallel(data.spawnCount, 32);

    }


}


public partial struct SetComponentJob : IJobParallelForBatch
{

    public NativeArray<Entity> entities;
    [NativeDisableParallelForRestriction] /*[ReadOnly] */public ComponentLookup<LinearMoveAuthoring.Data> linearMoveLookup;
    [NativeDisableParallelForRestriction] public ComponentLookup<LocalTransform> transformLookup;
    public int rows;
    public float offset;
    public float spacing;

    public void Execute(int startIndex, int count)
    {
        uint randomSeed = (uint)(startIndex + 1);// plus 1 to make seed non-zero.
        Unity.Mathematics.Random random = new(randomSeed);

        for (int i = startIndex; i < startIndex + count; i++)
        {
            Entity e = this.entities[i];
            this.SetLocalTransform(e, i);
            this.SetDirection(e, ref random);
        }

    }


    private void SetLocalTransform(Entity e, int index)
    {
        RefRW<LocalTransform> localTransform = this.transformLookup.GetRefRWOptional(e);
        if (!localTransform.IsValid)
        {
            Debug.LogError("LocalTransform not found");
            return;
        }


        int row = index / this.rows;
        int col = index % this.rows;

        localTransform.ValueRW.Position = GetGridPosition(col, row, spacing, offset);


        float3 GetGridPosition(int col, int row, float spacing, float offset)
        {
            return new
            (
                (col * spacing) - offset,
                0,
                (row * spacing) - offset
            );
        }
    }


    private void SetDirection(Entity e, ref Unity.Mathematics.Random random)
    {
        RefRW<LinearMoveAuthoring.Data> data = this.linearMoveLookup.GetRefRWOptional(e);
        if (!data.IsValid)
        {
            Debug.Log("data not found.");
            return;
        }


        data.ValueRW.direction = GetRandomDirection(ref random);


        float3 GetRandomDirection(ref Unity.Mathematics.Random random)
        {
            return new float3(
                random.NextFloat(-1f, 1f),
                0f,
                random.NextFloat(-1f, 1f));
        }
    }



}

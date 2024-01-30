using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct LinearMoveSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<LinearMoveAuthoring.Data>();
    }

    public void OnUpdate(ref SystemState state)
    {
        LinearMoveJob job = new()
        {
            deltaTime = SystemAPI.Time.DeltaTime,
        };

        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct LinearMoveJob : IJobEntity
{
    public float deltaTime;

    public void Execute(in LinearMoveAuthoring.Data data, ref LocalTransform transform)
    {
        transform = transform.Translate(data.direction * data.speed * this.deltaTime);
    }
}



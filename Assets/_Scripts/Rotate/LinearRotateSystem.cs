using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct LinearRotateSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<LinearRotateAuthoring.Data>();
    }

    public void OnUpdate(ref SystemState state)
    {
        LinearRotateJob job = new()
        {
            deltaTime = SystemAPI.Time.DeltaTime,
        };

        job.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct LinearRotateJob : IJobEntity
{
    public float deltaTime;

    public void Execute(in LinearRotateAuthoring.Data cubeData, ref LocalTransform transform)
    {
        //transform = transform.RotateY(math.radians(cubeData.speed * deltaTime));
        transform = transform.RotateX(math.radians(cubeData.speed * deltaTime * cubeData.direction.x));
        transform = transform.RotateY(math.radians(cubeData.speed * deltaTime * cubeData.direction.y));
        transform = transform.RotateZ(math.radians(cubeData.speed * deltaTime * cubeData.direction.z));
    }
}

//public partial class CubeSystem : SystemBase
//{
//    protected override void OnUpdate()
//    {
//        float deltaTime = SystemAPI.Time.DeltaTime;
//        foreach (var (cubeData, transform) in SystemAPI.Query<RefRO<CubeData>, RefRW<LocalTransform>>())
//        {
//            transform.ValueRW = transform.ValueRO.RotateY(math.radians(cubeData.ValueRO.speed * deltaTime));
//        }
//    }

//}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


public partial struct CubeSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        CubeJob job = new CubeJob
        {
            deltaTime = SystemAPI.Time.DeltaTime,
        };
        job.ScheduleParallel();
    }
}


public partial struct CubeJob : IJobEntity
{
    public float deltaTime;
    public void Execute(ref CubeData cubeData, ref LocalTransform transform)
    {
        transform = transform.RotateY(math.radians(cubeData.speed * deltaTime));
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

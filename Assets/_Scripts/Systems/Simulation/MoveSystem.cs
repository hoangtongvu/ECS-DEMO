using Components;
using Components.Misc.GlobalConfigs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct MoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    CanMoveEntityTag
                    , LocalTransform
                    , MoveDirectionFloat2
                    , MoveSpeedLinear>()
                .Build();

            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameGlobalConfigs = SystemAPI.GetSingleton<GameGlobalConfigsICD>();
            new MoveJob
            {
                deltaTime = SystemAPI.Time.DeltaTime,
                speedScale = gameGlobalConfigs.Value.EntityMoveSpeedScale,
            }.ScheduleParallel();
        }

        [WithAll(typeof(CanMoveEntityTag))]
        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            public float deltaTime;
            [ReadOnly] public float speedScale;

            private void Execute(
                ref LocalTransform transform
                , in MoveSpeedLinear speed
                , in MoveDirectionFloat2 direction
            )
            {
                float3 float3Dir = new (direction.Value.x, 0f, direction.Value.y);
                transform = transform.Translate(float3Dir * this.speedScale * speed.Value * this.deltaTime);
            }
        }
    }
}


using Components.GameEntity.Misc;
using Core.Utilities.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Utilities;

namespace Systems.Initialization.GameEntity.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct SetPosWithinRadiusCommandListExecutorSystem : ISystem
    {
        private Random rand;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var su = SingletonUtilities.GetInstance(state.EntityManager);
            su.AddOrSetComponentData(new SetPosWithinRadiusCommandList
            {
                Value = new(15, Allocator.Persistent),
            });

            this.rand = new Random(1);

            state.RequireForUpdate<SetPosWithinRadiusCommandList>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandList = SystemAPI.GetSingleton<SetPosWithinRadiusCommandList>();
            int commandCount = commandList.Value.Length;

            if (commandCount == 0) return;

            var randomSeeds = new NativeArray<uint>(commandCount, Allocator.TempJob);

            for (int i = 0; i < commandCount; i++)
            {
                randomSeeds[i] = this.rand.NextUInt();
            }

            // NOTE: Be sure that entity in each command is distinct
            state.Dependency = new SetPosJob
            {
                RandomSeeds = randomSeeds,
                CommandList = commandList,
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
            }.ScheduleParallel(commandCount, 64, state.Dependency);

            state.Dependency = new ClearCommandListJob
            {
                CommandList = commandList,
            }.Schedule(state.Dependency);

        }

        [BurstCompile]
        private struct SetPosJob : IJobParallelForBatch
        {
            [DeallocateOnJobCompletion]
            [ReadOnly]
            public NativeArray<uint> RandomSeeds;

            [ReadOnly]
            public SetPosWithinRadiusCommandList CommandList;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<LocalTransform> TransformLookup;

            [BurstCompile]
            public void Execute(int startIndex, int count)
            {
                Random rand = new(this.RandomSeeds[startIndex] + 1);
                float2 tempVector = new(1, 1);
                int upperBound = startIndex + count;

                for (int i = startIndex; i < upperBound; i++)
                {
                    var setPosCommand = this.CommandList.Value[i];

                    var transformRef = this.TransformLookup.GetRefRW(setPosCommand.BaseEntity);

                    transformRef.ValueRW.Position = this.GetRandomPositionInRadius(
                        in rand
                        , in tempVector
                        , in setPosCommand.Radius
                        , in setPosCommand.CenterPos);
                }

            }

            [BurstCompile]
            private float3 GetRandomPositionInRadius(
                in Random rand
                , in float2 tempVector
                , in float radius
                , in float3 centerPos)
            {
                float2 distanceVector = math.normalize(rand.NextFloat2(-tempVector, tempVector)) * radius;
                return centerPos.Add(x: distanceVector.x, z: distanceVector.y);
            }

        }

        [BurstCompile]
        private struct ClearCommandListJob : IJob
        {
            public SetPosWithinRadiusCommandList CommandList;

            [BurstCompile]
            public void Execute()
            {
                this.CommandList.Value.Clear();
            }

        }

    }

}
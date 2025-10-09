using Unity.Entities;
using Unity.Burst;
using Components.GameResource;
using Core.GameResource;
using Utilities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using Core.Utilities.Extensions;
using Components.GameResource.ItemPicking.Pickee.RePickUpCoolDown;

namespace Systems.Simulation.GameResource
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct ResourceItemSpawnCommandSystem : ISystem
    {
        private Random rand;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.rand = new Random(1);
            this.CreateCommandList(ref state);

            state.RequireForUpdate<ResourceItemEntityHolder>();
            state.RequireForUpdate<ResourceItemSpawnCommandList>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandList = SystemAPI.GetSingleton<ResourceItemSpawnCommandList>();
            if (commandList.Value.IsEmpty) return;

            int spawnCount = commandList.Value.Length;
            Entity prefabEntity = SystemAPI.GetSingleton<ResourceItemEntityHolder>().Value;

            var spawnCommandArray = commandList.Value.ToArray(state.WorldUpdateAllocator);
            var resourceItemEntityArray = state.EntityManager.Instantiate(prefabEntity, spawnCount, state.WorldUpdateAllocator);
            this.GetRandomSeedArray(spawnCount, out var randomSeedArray);

            state.Dependency = default;

            state.Dependency = new InitResourceItemJob
            {
                ResourceItemEntityArray = resourceItemEntityArray,
                SpawnCommandArray = spawnCommandArray,
                RandomSeedArray = randomSeedArray,
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                PhysicsVelocityLookup = SystemAPI.GetComponentLookup<PhysicsVelocity>(),
                ResourceItemICDLookup = SystemAPI.GetComponentLookup<ResourceItemICD>(),
                PreviousPickerEntityLookup = SystemAPI.GetComponentLookup<PreviousPickerEntity>(),
            }.ScheduleParallel(spawnCount, 20, state.Dependency);

            commandList.Value.Clear();
        }

        [BurstCompile]
        private void CreateCommandList(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new ResourceItemSpawnCommandList
                {
                    Value = new(100, Allocator.Persistent),
                });
        }

        [BurstCompile]
        private void GetRandomSeedArray(int count, out NativeArray<uint> randomSeedArray)
        {
            randomSeedArray = new(count, Allocator.TempJob);

            for (int i = 0; i < count; i++)
            {
                randomSeedArray[i] = this.rand.NextUInt() + 1;
            }
        }

    }

    [BurstCompile]
    public struct InitResourceItemJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<Entity> ResourceItemEntityArray;
        [ReadOnly] public NativeArray<ResourceItemSpawnCommand> SpawnCommandArray;

        [ReadOnly]
        [DeallocateOnJobCompletion]
        public NativeArray<uint> RandomSeedArray;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<LocalTransform> TransformLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<PhysicsVelocity> PhysicsVelocityLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<ResourceItemICD> ResourceItemICDLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<PreviousPickerEntity> PreviousPickerEntityLookup;

        [BurstCompile]
        public void Execute(int startIndex, int count)
        {
            int length = startIndex + count;
            var rand = new Random(this.RandomSeedArray[startIndex]);
            const float offsetY = 0.5f;
            const float speed = 3f;

            for (int i = startIndex; i < length; i++)
            {
                Entity entity = this.ResourceItemEntityArray[i];
                var spawnCommand = this.SpawnCommandArray[i];

                float2 randomDir2 = rand.NextFloat2Direction();
                float3 velocityDir3 = new(randomDir2.x, 1f, randomDir2.y);

                var transformRef = this.TransformLookup.GetRefRWOptional(entity);
                var velocityRef = this.PhysicsVelocityLookup.GetRefRWOptional(entity);
                var resourceItemICDRef = this.ResourceItemICDLookup.GetRefRWOptional(entity);
                var prevPickerEntityRef = this.PreviousPickerEntityLookup.GetRefRWOptional(entity);

                transformRef.ValueRW.Position = spawnCommand.SpawnPos.Add(y: offsetY);
                velocityRef.ValueRW.Linear = velocityDir3 * speed;

                resourceItemICDRef.ValueRW.ResourceType = spawnCommand.ResourceType;
                resourceItemICDRef.ValueRW.Quantity = spawnCommand.Quantity;

                prevPickerEntityRef.ValueRW.Value = spawnCommand.SpawnerEntity;
            }

        }

    }

}
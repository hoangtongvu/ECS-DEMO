using Unity.Entities;
using Unity.Burst;
using Components.GameResource;
using Core.GameResource;
using Utilities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Transforms;

namespace Systems.Simulation.GameResource
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct ResourceItemSpawnCommandSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.CreateCommandList(ref state);

            //var query0 = SystemAPI.QueryBuilder()
            //    .WithAll<
            //        HarvesteeProfileIdHolder
            //        , DropResourceHpThreshold
            //        , HarvesteeHealthChangedTag>()
            //    .Build();

            //state.RequireForUpdate(query0);
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

            state.Dependency = default;

            state.Dependency =
                new InitResourceItemJob
                {
                    ResourceItemEntityArray = resourceItemEntityArray,
                    SpawnCommandArray = spawnCommandArray,
                    TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                    ResourceItemICDLookup = SystemAPI.GetComponentLookup<ResourceItemICD>(),
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

    }

    [BurstCompile]
    public struct InitResourceItemJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<Entity> ResourceItemEntityArray;
        [ReadOnly] public NativeArray<ResourceItemSpawnCommand> SpawnCommandArray;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<LocalTransform> TransformLookup;

        [NativeDisableParallelForRestriction]
        public ComponentLookup<ResourceItemICD> ResourceItemICDLookup;

        [BurstCompile]
        public void Execute(int startIndex, int count)
        {
            int length = startIndex + count;

            for (int i = startIndex; i < length; i++)
            {
                Entity entity = this.ResourceItemEntityArray[i];
                var spawnCommand = this.SpawnCommandArray[i];

                var transformRef = this.TransformLookup.GetRefRWOptional(entity);
                var resourceItemICDRef = this.ResourceItemICDLookup.GetRefRWOptional(entity);

                transformRef.ValueRW.Position = spawnCommand.SpawnPos;
                resourceItemICDRef.ValueRW.ResourceType = spawnCommand.ResourceType;
                resourceItemICDRef.ValueRW.Quantity = spawnCommand.Quantity;
            }

        }
    }


}
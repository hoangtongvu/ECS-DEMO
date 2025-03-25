using Components.Misc.WorldMap;
using Core.Misc.WorldMap;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Misc.WorldMap
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct ChunkComponentsInitSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldMapChangedTag>();

            var su = SingletonUtilities.GetInstance(state.EntityManager);

            this.InitChunkList(ref state);
            this.InitChunkExitsContainer(ref state);
            this.InitChunkIndexToExitIndexesMap(ref state);
            this.InitChunkExitIndexesContainer(ref state);

            su.AddOrSetComponentData(new HighestExitCount
            {
                Value = int.MinValue,
            });

            state.Enabled = false;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        }

        [BurstCompile]
        private void InitChunkList(ref SystemState state)
        {
            NativeList<Chunk> chunkList = new(100, Allocator.Persistent);

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new ChunkList
                {
                    Value = chunkList,
                });

        }

        [BurstCompile]
        private void InitChunkExitsContainer(ref SystemState state)
        {
            NativeList<ChunkExit> chunkExitsContainer = new(400, Allocator.Persistent);

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new ChunkExitsContainer
                {
                    Value = chunkExitsContainer,
                });

        }

        [BurstCompile]
        private void InitChunkIndexToExitIndexesMap(ref SystemState state)
        {
            NativeList<ChunkExitIndexRange> chunkIndexToExitIndexesMap = new(100, Allocator.Persistent);

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new ChunkIndexToExitIndexesMap
                {
                    Value = chunkIndexToExitIndexesMap,
                });

        }

        [BurstCompile]
        private void InitChunkExitIndexesContainer(ref SystemState state)
        {
            NativeList<int> chunkExitIndexesContainer = new(400, Allocator.Persistent);

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new ChunkExitIndexesContainer
                {
                    Value = chunkExitIndexesContainer,
                });

        }

    }

}
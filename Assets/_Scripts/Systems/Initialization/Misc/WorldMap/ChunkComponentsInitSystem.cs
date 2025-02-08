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

            this.InitChunkList(ref state);
            this.InitChunkIndexToExitsMap(ref state);
            this.InitChunkExitsContainer(ref state);

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
        private void InitChunkIndexToExitsMap(ref SystemState state)
        {
            NativeList<ChunkExitRange> chunkIndexToExitsMap = new(100, Allocator.Persistent);

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new ChunkIndexToExitsMap
                {
                    Value = chunkIndexToExitsMap,
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

    }

}
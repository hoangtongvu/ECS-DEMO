using Unity.Entities;
using Unity.Burst;
using UnityEngine;
using Components.Misc.WorldMap;
using Core.Misc.WorldMap;
using Utilities;

namespace Systems.Presentation.Misc.WorldMap
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [BurstCompile]
    public partial class ChunksDrawSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<ChunkList>();
            this.RequireForUpdate<MapDebugConfig>();
            this.RequireForUpdate<ChunkDebugConfig>();
            this.RequireForUpdate<MapCellSize>();
        }

        protected override void OnStartRunning()
        {
            GameObject gameObject = new("ChunksDrawer");
            var chunksDrawer = gameObject.AddComponent<ChunksDrawer>();

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new ChunksDrawerRef
                {
                    Value = chunksDrawer,
                });

        }

        protected override void OnUpdate()
        {
            var mapDebugConfig = SystemAPI.GetSingleton<MapDebugConfig>();
            if (!mapDebugConfig.ShowChunks) return;

            var chunkList = SystemAPI.GetSingleton<ChunkList>().Value;
            var chunkDebugConfig = SystemAPI.GetSingleton<ChunkDebugConfig>();
            var chunksDrawer = SystemAPI.ManagedAPI.GetSingleton<ChunksDrawerRef>().Value;
            float mapCellSize = SystemAPI.GetSingleton<MapCellSize>().Value;

            if (!Input.GetKeyDown(KeyCode.Space)) return;

            ChunksDrawerConfig config = new()
            {
                CellSize = mapCellSize,
                Chunks = chunkList,
                ColorPalette = chunkDebugConfig.GridLineColorPalette,
            };

            chunksDrawer.ApplyConfig(in config);

        }

    }

}
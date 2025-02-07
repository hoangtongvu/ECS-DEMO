using Components.Misc.WorldMap;
using Core.Misc.WorldMap;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Utilities;

namespace Systems.Initialization.Misc.WorldMap
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class ChunkDebugConfigBakingSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.LoadConfigSO(out var configSO);

            var colorArray = new NativeArray<Color>(configSO.ChunkGridLineColors, Allocator.Persistent);

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new ChunkDebugConfig
                {
                    GridLineColorPalette = colorArray,
                });
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }

        private void LoadConfigSO(out ChunkDebugConfigSO configSO)
        {
            configSO = Resources.Load<ChunkDebugConfigSO>(ChunkDebugConfigSO.DefaultAssetPath);
        }

    }

}
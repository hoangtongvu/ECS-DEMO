using Components.Misc.WorldMap;
using Core.Misc;
using Unity.Entities;
using UnityEngine;

namespace Systems.Initialization.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SpawnSimpleBGMPlayerForSampleMapSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<SampleMapTag>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var go = new GameObject(nameof(SimpleBGMPlayer));
            var bgmPlayer = go.AddComponent<SimpleBGMPlayer>();

            bgmPlayer.TogglePlayMusic();
        }

    }

}
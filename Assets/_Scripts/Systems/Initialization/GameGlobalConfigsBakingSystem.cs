using Components.Misc.GlobalConfigs;
using Core.Misc.GlobalConfigs;
using Unity.Entities;
using UnityEngine;
using Utilities;


namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GameGlobalConfigsBakingSystem : SystemBase
    {

        protected override void OnCreate()
        {
            this.LoadConfigsSO(out var gameGlobalConfigsSO);

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new GameGlobalConfigsICD
                {
                    Value = gameGlobalConfigsSO.Configs,
                });
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }


        private void LoadConfigsSO(out GameGlobalConfigsSO gameGlobalConfigsSO)
        {
            gameGlobalConfigsSO = Resources.Load<GameGlobalConfigsSO>(GameGlobalConfigsSO.DefaultAssetPath);
        }
    }

}
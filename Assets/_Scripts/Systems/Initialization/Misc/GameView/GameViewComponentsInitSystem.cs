using Components.Misc.GameView;
using Components.MyCamera;
using Core.Misc.GameView;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Utilities;

namespace Systems.Initialization.Misc.GameView
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(GameViewChangedTagClearSystem))]
    [BurstCompile]
    public partial struct GameViewComponentsInitSystem : ISystem
    {
        //[BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.InitCurrentGameView(ref state);
            this.InitFixedAngleMap(ref state);
            this.InitPlayerViewCamOffset(ref state);

            state.RequireForUpdate<CameraEntityTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            var gameViewChangedTag = SystemAPI.GetSingletonRW<GameViewChangedTag>();
            gameViewChangedTag.ValueRW.Value = true;

        }

        //[BurstCompile]
        private void InitCurrentGameView(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new CurrentGameView
                {
                    Value = GameViewType.PlayerView,
                });

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddComponent<GameViewChangedTag>();

        }

        //[BurstCompile]
        private void InitFixedAngleMap(ref SystemState state)
        {
            int length = Enum.GetNames(typeof(GameViewType)).Length;
            NativeArray<float3> map = new(length, Allocator.Persistent);

            map[(int)GameViewType.PlayerView] = new(33, 0, 0);
            map[(int)GameViewType.FreeView] = new(90, 0, 0);

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new GameViewFixedAngleMap
                {
                    Value = map,
                });

        }

        //[BurstCompile]
        private void InitPlayerViewCamOffset(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new PlayerViewCamOffset
                {
                    Value = new(0, 9, -9),
                });

        }

    }

}
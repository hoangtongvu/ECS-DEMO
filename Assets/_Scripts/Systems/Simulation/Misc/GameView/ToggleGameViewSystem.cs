using Unity.Entities;
using UnityEngine;
using Components.Misc.GameView;
using System;
using Core.Misc.GameView;

namespace Systems.Simulation.Misc.GameView
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class ToggleGameViewSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    GameViewChangedTag
                    , CurrentGameView>()
                .Build();

            this.RequireForUpdate(query0);
            
        }

        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.F1)) return;

            var gameViewChangedTagRef = SystemAPI.GetSingletonRW<GameViewChangedTag>();
            gameViewChangedTagRef.ValueRW.Value = true;

            var currentGameViewRef = SystemAPI.GetSingletonRW<CurrentGameView>();

            int length = Enum.GetNames(typeof(GameViewType)).Length;
            int currentIndex = (int) currentGameViewRef.ValueRO.Value;
            int nextIndex = currentIndex + 1;

            if (nextIndex == length) nextIndex = 1;

            currentGameViewRef.ValueRW.Value = (GameViewType)nextIndex;

        }

    }

}
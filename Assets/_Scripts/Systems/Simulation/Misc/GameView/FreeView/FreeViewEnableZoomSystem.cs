using Unity.Entities;
using Components.Misc.GameView;
using Core.Misc.GameView;
using Unity.Burst;
using Utilities;

namespace Systems.Simulation.Misc.GameView.FreeView
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct FreeViewEnableZoomSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new CanZoomViewTag
                {
                    Value = false,
                });

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    GameViewChangedTag
                    , CurrentGameView
                    , GameViewFixedAngleMap>()
                .Build();

            state.RequireForUpdate(query0);
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            bool gameViewChanged = SystemAPI.GetSingleton<GameViewChangedTag>().Value;
            if (!gameViewChanged) return;

            var canZoomViewTag = SystemAPI.GetSingletonRW<CanZoomViewTag>();
            GameViewType currentGameView = SystemAPI.GetSingleton<CurrentGameView>().Value;

            if (currentGameView != GameViewType.FreeView)
            {
                canZoomViewTag.ValueRW.Value = false;
                return;
            }

            canZoomViewTag.ValueRW.Value = true;

        }

    }

}
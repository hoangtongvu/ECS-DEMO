using Unity.Entities;
using Components.Misc.GameView;
using Core.Misc.GameView;
using Components.Camera;
using Unity.Mathematics;
using Unity.Burst;
using Components.Misc.Tween;

namespace Systems.Simulation.Misc.GameView.PlayerView
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct SetViewPositionSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
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

            GameViewType currentGameView = SystemAPI.GetSingleton<CurrentGameView>().Value;
            if (currentGameView != GameViewType.PlayerView) return;

            float3 playerViewCamOffset = SystemAPI.GetSingleton<PlayerViewCamOffset>().Value;

            TweenData tweenData = new()
            {
                BaseSpeed = 2f,
                LifeTimeCounterSecond = 0f,
                Target = new()
                {
                    Float3 = playerViewCamOffset,
                },
            };

            foreach (var (addPosTweenDataRef, canAddPosTweenTag) in
                SystemAPI.Query<
                    RefRW<AddPosTweenData>
                    , EnabledRefRW<CanAddPosTweenTag>>()
                    .WithAll<CameraEntityTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                addPosTweenDataRef.ValueRW.Value = tweenData;
                canAddPosTweenTag.ValueRW = true;

            }

        }

    }

}
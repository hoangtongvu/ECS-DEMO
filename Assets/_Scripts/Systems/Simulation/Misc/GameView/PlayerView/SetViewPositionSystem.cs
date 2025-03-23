using Unity.Entities;
using Components.Misc.GameView;
using Core.Misc.GameView;
using Components.Camera;
using Unity.Mathematics;
using Unity.Burst;
using Utilities.Tweeners.Camera;

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

            foreach (var (addPosXZTweenDataRef, canAddPosXZTweenTag, addPosYTweenDataRef, canAddPosYTweenTag) in
                SystemAPI.Query<
                    RefRW<AddPosXZTweener_TweenData>
                    , EnabledRefRW<Can_AddPosXZTweener_TweenTag>
                    , RefRW<AddPosYTweener_TweenData>
                    , EnabledRefRW <Can_AddPosYTweener_TweenTag>>()
                    .WithAll<CameraEntityTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                AddPosXZTweener.TweenBuilder.Create()
                    .WithBaseSpeed(2f)
                    .WithTarget(playerViewCamOffset.xz)
                    .Build(ref addPosXZTweenDataRef.ValueRW, canAddPosXZTweenTag);

                AddPosYTweener.TweenBuilder.Create()
                    .WithBaseSpeed(2f)
                    .WithTarget(playerViewCamOffset.y)
                    .Build(ref addPosYTweenDataRef.ValueRW, canAddPosYTweenTag);

            }

        }

    }

}
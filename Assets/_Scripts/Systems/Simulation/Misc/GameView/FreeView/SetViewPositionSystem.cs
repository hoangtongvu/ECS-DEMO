using Unity.Entities;
using Components.Misc.GameView;
using Core.Misc.GameView;
using Unity.Mathematics;
using Unity.Burst;
using TweenLib.Utilities;
using Components.MyCamera;
using Utilities.Tweeners.MyCamera;

namespace Systems.Simulation.Misc.GameView.FreeView
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
            if (currentGameView != GameViewType.FreeView) return;

            const float defaultOffsetY = 15;

            foreach (var (addPosYTweenDataRef, canAddPosYTweenTag, addPosXZTweenDataRef, canAddPosXZTweenTag) in
                SystemAPI.Query<
                    RefRW<AddPosYTweener_TweenData>
                    , EnabledRefRW<Can_AddPosYTweener_TweenTag>
                    , RefRW<AddPosXZTweener_TweenData>
                    , EnabledRefRW < Can_AddPosXZTweener_TweenTag >> ()
                    .WithAll<CameraEntityTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                AddPosYTweener.TweenBuilder.Create(0.8f, defaultOffsetY)
                    .WithEase(EasingType.EaseOutQuad)
                    .Build(ref addPosYTweenDataRef.ValueRW, canAddPosYTweenTag);

                AddPosXZTweener.TweenBuilder.Create(0.8f, float2.zero)
                    .WithEase(EasingType.EaseOutQuad)
                    .Build(ref addPosXZTweenDataRef.ValueRW, canAddPosXZTweenTag);

            }

        }

    }

}
using Unity.Entities;
using Components.Misc.GameView;
using Core.Misc.GameView;
using Components.Camera;
using Unity.Mathematics;
using Unity.Burst;
using Utilities.Tweeners.Camera;

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

            foreach (var (addPosTweenDataRef, canAddPosTweenTag) in
                SystemAPI.Query<
                    RefRW<AddPosTweener_TweenData>
                    , EnabledRefRW<Can_AddPosTweener_TweenTag>>()
                    .WithAll<CameraEntityTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                AddPosTweener.TweenBuilder.Create()
                    .WithBaseSpeed(2f)
                    .WithTarget(new float3(0, defaultOffsetY, 0))
                    .Build(ref addPosTweenDataRef.ValueRW, canAddPosTweenTag);

            }

        }

    }

}
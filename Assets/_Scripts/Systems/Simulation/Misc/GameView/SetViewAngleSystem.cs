using Unity.Entities;
using Components.Misc.GameView;
using Core.Misc.GameView;
using Components.Camera;
using Unity.Mathematics;
using Unity.Transforms;
using TweenLib.StandardTweeners;

namespace Systems.Simulation.Misc.GameView
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct SetViewAngleSystem : ISystem
    {
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

        public void OnUpdate(ref SystemState state)
        {
            bool gameViewChanged = SystemAPI.GetSingleton<GameViewChangedTag>().Value;
            if (!gameViewChanged) return;

            GameViewType currentGameView = SystemAPI.GetSingleton<CurrentGameView>().Value;
            var angleMap = SystemAPI.GetSingleton<GameViewFixedAngleMap>().Value;

            float3 newAngle = angleMap[(int)currentGameView];

            RefRW<TransformRotationTweener_TweenData> quaternionTweenDataRef = default;
            EnabledRefRW<Can_TransformRotationTweener_TweenTag> canQuaternionTweenTag = default;

            foreach (var item in
                SystemAPI.Query<
                    RefRW<TransformRotationTweener_TweenData>
                    , EnabledRefRW<Can_TransformRotationTweener_TweenTag>>()
                    .WithAll<CameraEntityTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                quaternionTweenDataRef = item.Item1;
                canQuaternionTweenTag = item.Item2;
            }

            TransformRotationTweener.TweenBuilder.Create()
                .WithBaseSpeed(2f)
                .WithTarget(quaternion.EulerXYZ(math.radians(newAngle)).value)
                .Build(ref quaternionTweenDataRef.ValueRW, canQuaternionTweenTag);

        }

    }

}
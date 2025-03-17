using Unity.Entities;
using UnityEngine;
using Components.Misc.GameView;
using Components.Camera;
using Utilities.Tweeners.Camera;

namespace Systems.Simulation.Misc.GameView.FreeView
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ViewZoomSystem : SystemBase
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
            bool canZoomView = SystemAPI.GetSingleton<CanZoomViewTag>().Value;
            if (!canZoomView) return;

            float scrollAxis = Input.GetAxis("Mouse ScrollWheel");

            if (scrollAxis == 0) return;

            const float zoomSpeed = 5000f; // TODO: Extract this into component.

            foreach (var (addPosTweenDataRef, canAddPosTweenTag) in
                SystemAPI.Query<
                    RefRW<AddPosYTweener_TweenData>
                    , EnabledRefRW<Can_AddPosYTweener_TweenTag>>()
                    .WithAll<CameraEntityTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                AddPosYTweener.TweenBuilder.Create()
                    .WithBaseSpeed(2f)
                    .WithTarget(addPosTweenDataRef.ValueRO.Target - scrollAxis * zoomSpeed * SystemAPI.Time.DeltaTime)
                    .Build(ref addPosTweenDataRef.ValueRW, canAddPosTweenTag);

            }

        }

    }

}
using Unity.Entities;
using UnityEngine;
using Components.Misc.GameView;
using Components.Camera;
using Core.Utilities.Extensions;

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

            var addPosRef = SystemAPI.GetSingletonRW<AddPos>();
            const float zoomSpeed = 1000f; // TODO: Extract this into component.

            addPosRef.ValueRW.Value = addPosRef.ValueRO.Value.Add(y: -scrollAxis * zoomSpeed * SystemAPI.Time.DeltaTime);

        }

    }

}
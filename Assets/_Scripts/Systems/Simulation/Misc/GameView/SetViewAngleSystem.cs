using Unity.Entities;
using Components.Misc.GameView;
using Core.Misc.GameView;
using Components.Camera;
using Unity.Mathematics;
using Unity.Transforms;

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

            RefRW<LocalTransform> transformRef = default;

            foreach (var item in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<CameraEntityTag>())
            {
                transformRef = item;
            }

            transformRef.ValueRW = transformRef.ValueRW.WithRotation(quaternion.EulerXYZ(math.radians(newAngle)));

        }

    }

}
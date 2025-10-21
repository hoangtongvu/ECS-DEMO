using Cinemachine;
using Components.GameState;
using Components.MyCamera;
using LitMotion;
using Unity.Entities;
using Utilities;

namespace Systems.Simulation.MyCamera
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ChangeCamFOVBasedOnGameStateSystem : SystemBase
    {
        private MotionHandle fovMotionHandle;

        protected override void OnCreate()
        {
            var camQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    MainVirtualCamHolder
                    , OriginalCamFOV
                    , GameOverFOVScale>()
                .Build();

            this.RequireForUpdate(camQuery);
            this.RequireForUpdate<GameOverEvent>();
        }

        protected override void OnUpdate()
        {
            var su = SingletonUtilities.GetInstance(this.EntityManager);
            if (!su.IsComponentEnabled<GameOverEvent>()) return;

            var virtualCamera = SystemAPI.GetSingleton<MainVirtualCamHolder>().Value.Value;
            float originalFOV = SystemAPI.GetSingleton<OriginalCamFOV>().Value;
            float gameOverFOVScale = SystemAPI.GetSingleton<GameOverFOVScale>();

            this.ChangeVirtualCamFOV(virtualCamera, originalFOV * gameOverFOVScale);
        }

        private void ChangeVirtualCamFOV(
            CinemachineVirtualCamera virtualCamera
            , float targetFOV)
        {
            this.fovMotionHandle.TryCancel();

            this.fovMotionHandle = LMotion.Create(virtualCamera.m_Lens.FieldOfView, targetFOV, 2f)
                .WithEase(Ease.InOutSine)
                .Bind(tempFOV => virtualCamera.m_Lens.FieldOfView = tempFOV);
        }

    }

}
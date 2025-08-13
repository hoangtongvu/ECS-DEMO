using Cinemachine;
using Components.GameEntity.Reaction;
using Components.MyCamera;
using Components.Player;
using LitMotion;
using Unity.Entities;

namespace Systems.Simulation.MyCamera
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class ChangeCamFOVBasedOnMovementSystem : SystemBase
    {
        private MotionHandle fovMotionHandle;
        private EntityQuery playerIdleQuery;
        private EntityQuery playerWalkQuery;
        private EntityQuery playerRunQuery;

        protected override void OnCreate()
        {
            var camQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    MainVirtualCamHolder
                    , OriginalCamFOV
                    , RunningFOVScale>()
                .Build();

            this.playerIdleQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , IdleReaction.StartedTag>()
                .Build();

            this.playerWalkQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , WalkReaction.StartedTag>()
                .Build();

            this.playerRunQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , RunReaction.StartedTag>()
                .Build();

            this.RequireForUpdate(camQuery);
            this.RequireForUpdate(this.playerIdleQuery);
            this.RequireForUpdate(this.playerWalkQuery);
            this.RequireForUpdate(this.playerRunQuery);
        }

        protected override void OnUpdate()
        {
            var virtualCamera = SystemAPI.GetSingleton<MainVirtualCamHolder>().Value.Value;
            float originalFOV = SystemAPI.GetSingleton<OriginalCamFOV>().Value;
            float fovScaleOnRunning = SystemAPI.GetSingleton<RunningFOVScale>().Value;

            bool startedIdling = this.playerIdleQuery.CalculateEntityCount() != 0;
            bool startedWalking = this.playerWalkQuery.CalculateEntityCount() != 0;
            if (startedIdling || startedWalking)
            {
                this.ChangeVirtualCamFOV(virtualCamera, originalFOV);
                return;
            }

            bool startedRunning = this.playerRunQuery.CalculateEntityCount() != 0;
            if (startedRunning)
            {
                this.ChangeVirtualCamFOV(virtualCamera, originalFOV * fovScaleOnRunning);
                return;
            }

        }

        private void ChangeVirtualCamFOV(
            CinemachineVirtualCamera virtualCamera
            , float targetFOV)
        {
            this.fovMotionHandle.TryCancel();
            this.fovMotionHandle = LMotion.Create(virtualCamera.m_Lens.FieldOfView, targetFOV, 1f)
                .WithEase(Ease.InOutSine)
                .Bind(tempFOV => virtualCamera.m_Lens.FieldOfView = tempFOV);
        }

    }

}
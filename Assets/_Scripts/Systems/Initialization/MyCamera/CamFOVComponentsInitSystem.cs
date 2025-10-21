using Unity.Entities;
using Utilities;
using Components.MyCamera;

namespace Systems.Initialization.MyCamera
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class CamFOVComponentsInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<MainVirtualCamHolder>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var virtualCamera = SystemAPI.GetSingleton<MainVirtualCamHolder>().Value.Value;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            su.AddOrSetComponentData(new OriginalCamFOV
            {
                Value = virtualCamera.m_Lens.FieldOfView,
            });

            su.AddOrSetComponentData(new RunningFOVScale
            {
                Value = 1.1f,
            });

            su.AddOrSetComponentData(new GameOverFOVScale(0.6f));
        }

    }

}
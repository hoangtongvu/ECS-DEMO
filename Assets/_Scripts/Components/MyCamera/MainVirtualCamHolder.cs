using Unity.Entities;
using Cinemachine;

namespace Components.MyCamera
{
    public struct MainVirtualCamHolder : IComponentData
    {
        public UnityObjectRef<CinemachineVirtualCamera> Value;
    }
}

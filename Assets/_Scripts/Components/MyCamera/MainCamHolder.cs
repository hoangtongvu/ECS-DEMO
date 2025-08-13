using Unity.Entities;

namespace Components.MyCamera
{
    public struct MainCamHolder : IComponentData
    {
        public UnityObjectRef<UnityEngine.Camera> Value;
    }
}

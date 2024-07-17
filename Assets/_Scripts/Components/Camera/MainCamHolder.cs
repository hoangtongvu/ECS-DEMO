using Unity.Entities;

namespace Components.Camera
{
    public struct MainCamHolder : IComponentData
    {
        public UnityObjectRef<UnityEngine.Camera> Value;
    }

    
}

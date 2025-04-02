using Core.GameResource;
using Unity.Entities;

namespace Components.GameResource
{
    public struct ResourceProfilesSOHolder : IComponentData
    {
        public UnityObjectRef<ResourceProfilesSO> Value;
    }

}

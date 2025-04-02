using Core.Unit;
using Unity.Entities;

namespace Components.Unit
{
    public struct UnitProfilesSOHolder : IComponentData
    {
        public UnityObjectRef<UnitProfilesSO> Value;
    }

}

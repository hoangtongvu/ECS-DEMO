using Core.Unit.DarkUnit;
using Unity.Entities;

namespace Components.Unit.DarkUnit
{
    public struct DarkUnitConfigsSOHolder : IComponentData
    {
        public UnityObjectRef<DarkUnitConfigsSO> Value;
    }
}

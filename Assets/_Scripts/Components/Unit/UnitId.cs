using Core.Unit;
using Unity.Entities;

namespace Components.Unit
{
    public struct UnitId : IComponentData
    {
        public UnitType UnitType;
        public ushort LocalIndex;
    }

}

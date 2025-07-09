using Components.GameEntity.Misc;
using Unity.Entities;

namespace Components.Unit.DarkUnit
{
    public struct DarkUnitFactionIndexHolder : IComponentData
    {
        public FactionIndex Value;
    }
}

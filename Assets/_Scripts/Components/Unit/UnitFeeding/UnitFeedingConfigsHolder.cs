using Core.Unit.UnitFeeding;
using Unity.Entities;

namespace Components.Unit.UnitFeeding
{
    public struct UnitFeedingConfigsHolder : IComponentData
    {
        public UnitFeedingConfigs Value;
    }
}

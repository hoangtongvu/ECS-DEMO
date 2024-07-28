using Core.Harvest;
using Unity.Entities;

namespace Components.Harvest
{
    public struct HarvesteeHealthId : IComponentData
    {
        public HealthId Value;
    }
}

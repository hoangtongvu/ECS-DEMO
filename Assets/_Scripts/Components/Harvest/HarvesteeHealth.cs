using Core.Harvest;
using Unity.Collections;
using Unity.Entities;

namespace Components.Harvest
{
    public struct HarvesteeHealthId : IComponentData
    {
        public HealthId Value;
    }

    public struct HarvesteeHealthMap : IComponentData
    {
        public NativeHashMap<HealthId, uint> Value;
    }

    public struct HarvesteeHealthChangedTag : IComponentData, IEnableableComponent
    {

    }

    public struct DropResourceHpThreshold : IComponentData
    {
        public uint Value;
    }
}

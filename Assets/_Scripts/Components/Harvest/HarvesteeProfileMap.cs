using Core.Harvest;
using Unity.Collections;
using Unity.Entities;

namespace Components.Harvest
{
    public struct HarvesteeProfileMap : IComponentData
    {
        public NativeHashMap<HarvesteeProfileId, HarvesteeProfile> Value;
    }

}

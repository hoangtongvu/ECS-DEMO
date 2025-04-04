using Core.Harvest;
using Unity.Collections;
using Unity.Entities;

namespace Components.Harvest
{
    public struct HarvesteePrefabEntityMap : IComponentData
    {
        public NativeHashMap<HarvesteeProfileId, Entity> Value;
    }

}

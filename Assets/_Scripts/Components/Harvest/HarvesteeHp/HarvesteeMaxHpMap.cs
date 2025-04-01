using Unity.Collections;
using Unity.Entities;

namespace Components.Harvest.HarvesteeHp
{
    public struct HarvesteeMaxHpMap : IComponentData
    {
        public NativeHashMap<Entity, uint> Value;
    }

}

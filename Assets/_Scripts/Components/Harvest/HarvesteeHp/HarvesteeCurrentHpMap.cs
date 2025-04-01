using Unity.Collections;
using Unity.Entities;

namespace Components.Harvest.HarvesteeHp
{
    public struct HarvesteeCurrentHpMap : IComponentData
    {
        public NativeHashMap<Entity, uint> Value;
    }

}

using Core.Harvest;
using Unity.Collections;
using Unity.Entities;

namespace Components.Harvest
{
    public struct HarvesteeResourcceDropInfoMap : IComponentData
    {
        public NativeHashMap<Entity, ResourceDropInfo> Value;
    }

}

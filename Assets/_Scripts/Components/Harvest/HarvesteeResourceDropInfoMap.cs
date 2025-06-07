using Core.Harvest;
using Unity.Collections;
using Unity.Entities;

namespace Components.Harvest
{
    public struct HarvesteeResourceDropInfoMap : IComponentData
    {
        public NativeHashMap<Entity, ResourceDropInfo> Value;
    }

}

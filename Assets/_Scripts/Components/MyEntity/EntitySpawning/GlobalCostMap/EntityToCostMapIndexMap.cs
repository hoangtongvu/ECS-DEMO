using Unity.Collections;
using Unity.Entities;

namespace Components.MyEntity.EntitySpawning.GlobalCostMap
{
    public struct EntityToCostMapIndexMap : IComponentData
    {
        public NativeHashMap<Entity, int> Value;
    }

}

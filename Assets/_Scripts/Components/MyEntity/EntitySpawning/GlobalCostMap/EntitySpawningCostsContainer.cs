using Unity.Collections;
using Unity.Entities;

namespace Components.MyEntity.EntitySpawning.GlobalCostMap
{
    public struct EntitySpawningCostsContainer : IComponentData
    {
        public NativeList<uint> Value;
    }

}

using Unity.Collections;
using Unity.Entities;

namespace Components.MyEntity.EntitySpawning.GlobalCostMap.Containers
{
    public struct EntitySpawningDurationsContainer : IComponentData
    {
        public NativeList<float> Value;
    }

}

using Unity.Collections;
using Unity.Entities;

namespace Components.MyEntity.EntitySpawning.SpawningProfiles
{
    public struct EntitySpawningCostsContainer : IComponentData
    {
        public NativeList<uint> Value;
    }

}

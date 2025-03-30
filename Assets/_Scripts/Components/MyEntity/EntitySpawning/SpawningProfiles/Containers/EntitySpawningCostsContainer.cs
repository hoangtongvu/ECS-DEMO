using Unity.Collections;
using Unity.Entities;

namespace Components.MyEntity.EntitySpawning.SpawningProfiles.Containers
{
    public struct EntitySpawningCostsContainer : IComponentData
    {
        public NativeList<uint> Value;
    }

}

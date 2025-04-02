using Unity.Collections;
using Unity.Entities;

namespace Components.GameEntity.EntitySpawning.SpawningProfiles.Containers
{
    public struct EntitySpawningCostsContainer : IComponentData
    {
        public NativeList<uint> Value;
    }

}

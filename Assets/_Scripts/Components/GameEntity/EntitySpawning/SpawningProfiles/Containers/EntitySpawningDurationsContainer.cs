using Unity.Collections;
using Unity.Entities;

namespace Components.GameEntity.EntitySpawning.SpawningProfiles.Containers
{
    public struct EntitySpawningDurationsContainer : IComponentData
    {
        public NativeList<float> Value;
    }

}

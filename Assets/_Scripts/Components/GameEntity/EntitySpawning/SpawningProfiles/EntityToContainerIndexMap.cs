using Unity.Collections;
using Unity.Entities;

namespace Components.GameEntity.EntitySpawning.SpawningProfiles
{
    public struct EntityToContainerIndexMap : IComponentData
    {
        public NativeHashMap<Entity, int> Value;
    }

}

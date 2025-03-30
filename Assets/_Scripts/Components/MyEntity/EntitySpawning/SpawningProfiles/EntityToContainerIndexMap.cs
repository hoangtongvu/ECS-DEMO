using Unity.Collections;
using Unity.Entities;

namespace Components.MyEntity.EntitySpawning.SpawningProfiles
{
    public struct EntityToContainerIndexMap : IComponentData
    {
        public NativeHashMap<Entity, int> Value;
    }

}

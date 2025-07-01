using Unity.Collections;
using Unity.Entities;

namespace Components.GameEntity.EntitySpawning
{
    public struct SpawnedEntityArray : IComponentData
    {
        public NativeArray<Entity> Value;
    }

}

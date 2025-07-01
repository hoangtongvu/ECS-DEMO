using Unity.Entities;

namespace Components.GameEntity.EntitySpawning
{
    [InternalBufferCapacity(5)]
    public struct SpawnedEntityElement : IBufferElementData
    {
        public Entity Value;
    }

}

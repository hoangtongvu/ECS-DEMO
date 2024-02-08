using Unity.Entities;

namespace Components
{
    public struct EntitySpawner : IComponentData
    {
        public Entity prefab;
        public int spawnCount;
        public float spacing;
    }

    // 0 capacity means the buffer will be allocated outside of the chunk
    [InternalBufferCapacity(0)]
    public struct EntityRefElement : IBufferElementData
    {
        public Entity entity;
    }
}

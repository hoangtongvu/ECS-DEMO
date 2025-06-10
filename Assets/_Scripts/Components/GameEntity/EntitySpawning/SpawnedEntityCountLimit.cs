using Unity.Entities;

namespace Components.GameEntity.EntitySpawning
{
    public struct SpawnedEntityCountLimit : IComponentData
    {
        public byte Value;
    }

}

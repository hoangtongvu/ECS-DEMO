using Unity.Entities;

namespace Components.GameEntity.EntitySpawning
{
    public struct NewlySpawnedTag : IComponentData, IEnableableComponent
    {
    }

    public struct SpawnerEntityRef : IComponentData
    {
        public Entity Value;
    }

}
